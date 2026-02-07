import { MsalProvider } from "@azure/msal-react";
import {
  EventType,
  type EventMessage,
  type AuthenticationResult,
} from "@azure/msal-browser";
import { msalInstance } from "../utils/authConfig";
import type { ReactNode } from "react";

// Set active account on login
msalInstance.addEventCallback((event: EventMessage) => {
  if (event.eventType === EventType.LOGIN_SUCCESS && event.payload) {
    const payload = event.payload as AuthenticationResult;
    msalInstance.setActiveAccount(payload.account);
  }
});

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider = ({ children }: AuthProviderProps) => {
  return <MsalProvider instance={msalInstance}>{children}</MsalProvider>;
};
