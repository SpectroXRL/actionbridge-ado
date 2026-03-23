import { useCallback } from "react";
import { useMsal } from "@azure/msal-react";
import { apiRequest } from "./authConfig";

export const useApi = () => {
  const { instance, accounts } = useMsal();

  // useCallback keeps the same function reference unless instance or accounts change
  const getAccessToken = useCallback(async (): Promise<string> => {
    const account = accounts[0];
    if (!account) {
      throw new Error("No active account");
    }

    try {
      const response = await instance.acquireTokenSilent({
        ...apiRequest,
        account,
      });
      return response.accessToken;
    } catch (error) {
      // If silent token acquisition fails, try popup
      console.log(error);
      const response = await instance.acquireTokenPopup(apiRequest);
      return response.accessToken;
    }
  }, [instance, accounts]);

  // callApi depends on getAccessToken, so it goes in the dependency array
  const callApi = useCallback(
    async <T,>(endpoint: string, options: RequestInit = {}): Promise<T> => {
      const token = await getAccessToken();

      const response = await fetch(`http://localhost:5277${endpoint}`, {
        ...options,
        headers: {
          ...options.headers,
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      });

      if (!response.ok) {
        throw new Error(`API call failed: ${response.statusText}`);
      }

      return response.json();
    },
    [getAccessToken],
  );

  return { callApi, getAccessToken };
};
