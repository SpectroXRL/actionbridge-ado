import { useState } from "react";
import WorkItemsForm from "./components/ado/WorkItemsForm";
import type { WorkItemRequest } from "./types/WorkItemRequest";
import "./App.css";
import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
} from "@azure/msal-react";
import { SignInButton } from "./components/auth/SignInButton";
import { UserProfile } from "./components/auth/UserProfile";
import TranscriptFileUpload from "./components/ado/TranscriptFileUpload";
import TestFileUpload from "./components/ado/TestFileUpload";

const App = () => {
  const [workItems, setWorkItems] = useState<WorkItemRequest[]>([]);
  const [message, setMessage] = useState<string | null>(null);
  const [isError, setIsError] = useState(false);

  const modifyMessage = (newMessage: string | null) => setMessage(newMessage);
  const modifyWorkItems = (newWorkItems: WorkItemRequest[]) =>
    setWorkItems(newWorkItems);
  const modifyIsError = (newIsError: boolean) => setIsError(newIsError);

  return (
    <div className="app-container">
      <header className="app-header">
        <h1>ActionBridge - Azure DevOps Work Item Generator</h1>
        <AuthenticatedTemplate>
          <UserProfile />
        </AuthenticatedTemplate>
        <UnauthenticatedTemplate>
          <SignInButton />
        </UnauthenticatedTemplate>
      </header>

      <AuthenticatedTemplate>
        <TranscriptFileUpload
          setMessage={modifyMessage}
          setIsError={modifyIsError}
          setWorkItems={modifyWorkItems}
        />

        {import.meta.env.DEV && (
          <TestFileUpload
            setMessage={modifyMessage}
            setIsError={modifyIsError}
            setWorkItems={modifyWorkItems}
          />
        )}

        {message && (
          <div className={`message ${isError ? "error" : "success"}`}>
            {message}
          </div>
        )}

        <WorkItemsForm
          workItems={workItems}
          setWorkItems={setWorkItems}
          setMessage={modifyMessage}
          setIsError={modifyIsError}
        />
      </AuthenticatedTemplate>

      <UnauthenticatedTemplate>
        <div className="login-prompt">
          <p>Please sign in with your Microsoft account to continue.</p>
        </div>
      </UnauthenticatedTemplate>
    </div>
  );
};

export default App;
