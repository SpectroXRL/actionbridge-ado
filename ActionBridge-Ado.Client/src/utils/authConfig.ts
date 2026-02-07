import { type Configuration, LogLevel, PublicClientApplication } from "@azure/msal-browser";

const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_AZURE_AD_CLIENT_ID,
    authority: `https://login.microsoftonline.com/${import.meta.env.VITE_AZURE_AD_TENANT_ID}`,
    redirectUri: import.meta.env.VITE_REDIRECT_URI || "http://localhost:5173",
    postLogoutRedirectUri: "/",
  },
  cache: {
    cacheLocation: "sessionStorage",
  },
  system: {
    loggerOptions: {
      loggerCallback: (level, message, containsPii) => {
        if (containsPii) return;
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            break;
          case LogLevel.Warning:
            console.warn(message);
            break;
          case LogLevel.Info:
            console.info(message);
            break;
          case LogLevel.Verbose:
            console.debug(message);
            break;
        }
      },
    },
    allowPlatformBroker: false,
  },
};

// Scopes for your API
export const loginRequest = {
  scopes: ["User.Read"],
};

// Scopes for Azure DevOps (will be requested when calling API)
export const apiRequest = {
  scopes: [`api://${import.meta.env.VITE_AZURE_AD_CLIENT_ID}/access_as_user`],
};

// Azure DevOps scope for On-Behalf-Of flow
export const adoScopes = ["499b84ac-1321-427f-aa17-267ca6975798/user_impersonation"];

export const msalInstance = new PublicClientApplication(msalConfig);