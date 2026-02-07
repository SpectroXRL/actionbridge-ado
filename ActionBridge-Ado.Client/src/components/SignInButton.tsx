import { useMsal } from "@azure/msal-react";
import { loginRequest } from "../utils/authConfig";

export const SignInButton = () => {
  const { instance } = useMsal();

  const handleLogin = () => {
    instance.loginRedirect(loginRequest).catch((e) => {
      console.error("Login failed:", e);
    });
  };

  return (
    <button onClick={handleLogin} className="sign-in-btn">
      Sign in with Microsoft
    </button>
  );
};
