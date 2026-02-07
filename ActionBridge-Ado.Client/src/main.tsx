import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { AuthProvider } from "./components/AuthProvider.tsx";
import App from "./App.tsx";
import { msalInstance } from "./utils/authConfig.ts";

msalInstance.initialize().then(async () => {
  await msalInstance.handleRedirectPromise().catch((error) => {
    console.error("Redirect error:", error);
  });

  if (window.opener && window.opener !== window) {
    return;
  }

  createRoot(document.getElementById("root")!).render(
    <StrictMode>
      <AuthProvider>
        <App />
      </AuthProvider>
    </StrictMode>,
  );
});
