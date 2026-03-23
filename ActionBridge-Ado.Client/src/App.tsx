import { useEffect, useState, type ChangeEvent } from "react";
import WorkItemsForm from "./components/ado/WorkItemsForm";
import type { WorkItemRequest } from "./types/WorkItemRequest";
import "./App.css";
import {
  AuthenticatedTemplate,
  UnauthenticatedTemplate,
  useIsAuthenticated,
} from "@azure/msal-react";
import { SignInButton } from "./components/auth/SignInButton";
import { UserProfile } from "./components/auth/UserProfile";
import TranscriptFileUpload from "./components/ado/TranscriptFileUpload";
import TestFileUpload from "./components/ado/TestFileUpload";
import { useApi } from "./utils/useApi";

interface Project {
  id: string;
  name: string;
}
const App = () => {
  const [projects, setProjects] = useState<Project[]>([]);
  const [workItems, setWorkItems] = useState<WorkItemRequest[]>([]);
  const [message, setMessage] = useState<string | null>(null);
  const [isError, setIsError] = useState(false);
  const [selectedProject, setSelectedProject] = useState("");
  const [isProjectsLoading, setIsProjectsLoading] = useState(false);
  const organizationUrl = import.meta.env.VITE_ORGANIZATION_URL;

  const modifyMessage = (newMessage: string | null) => setMessage(newMessage);
  const modifyWorkItems = (newWorkItems: WorkItemRequest[]) =>
    setWorkItems(newWorkItems);
  const modifyIsError = (newIsError: boolean) => setIsError(newIsError);

  const handleProjectSelection = async (e: ChangeEvent<HTMLSelectElement>) => {
    setSelectedProject(e.target.value);
  };

  const isAuthenticated = useIsAuthenticated();
  const { getAccessToken } = useApi();

  useEffect(() => {
    if (!isAuthenticated) return;

    async function fetchProjects() {
      try {
        setIsProjectsLoading(true);
        const token = await getAccessToken();
        const response = await fetch(
          `http://localhost:5277/api/ado/projects?organizationUrl=${encodeURIComponent(organizationUrl)}`,
          {
            method: "GET",
            headers: {
              Authorization: `Bearer ${token}`,
            },
          },
        );

        const fetchedProjects: Project[] = await response.json();
        setProjects(fetchedProjects);
        if (fetchedProjects.length > 0) {
          setSelectedProject(fetchedProjects[0].name);
        }
      } catch (error) {
        console.log(error);
      } finally {
        setIsProjectsLoading(false);
      }
    }
    fetchProjects();
  }, [isAuthenticated, getAccessToken, organizationUrl]);

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
        {isProjectsLoading ? (
          <p>Loading Projects...</p>
        ) : (
          <select value={selectedProject} onChange={handleProjectSelection}>
            {projects.map((project) => (
              <option key={project.id} value={project.name}>
                {project.name}
              </option>
            ))}
          </select>
        )}

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
          selectedProject={selectedProject}
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
