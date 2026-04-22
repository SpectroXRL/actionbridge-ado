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
import { useApi } from "./utils/useApi";
import TestFileUpload from "./components/ado/TestFileUpload";

const apiUrl = import.meta.env.VITE_API_URL;

interface Organization {
  organization: Org;
  projects: Project[];
}

interface Org {
  accountId: string;
  accountURI: string;
  accountName: string;
}

interface Project {
  id: string;
  name: string;
}
const App = () => {
  const [organizations, setOrganizations] = useState<Organization[]>([]);
  const [workItems, setWorkItems] = useState<WorkItemRequest[]>([]);
  const [message, setMessage] = useState<string | null>(null);
  const [isError, setIsError] = useState(false);
  const [selectedProject, setSelectedProject] = useState("");
  const [selectedOrganization, setSelectedOrganization] = useState("");
  const [isOrganizationsLoading, setIsOrganizationsLoading] = useState(false);

  const modifyMessage = (newMessage: string | null) => setMessage(newMessage);
  const modifyWorkItems = (newWorkItems: WorkItemRequest[]) =>
    setWorkItems(newWorkItems);
  const modifyIsError = (newIsError: boolean) => setIsError(newIsError);

  const handleProjectSelection = async (e: ChangeEvent<HTMLSelectElement>) => {
    setSelectedProject(e.target.value);
  };

  const handleOrganizationSelection = async (
    e: ChangeEvent<HTMLSelectElement>,
  ) => {
    const newOrgName = e.target.value;
    setSelectedOrganization(newOrgName);

    const org = organizations.find(
      (org) => org.organization.accountName === newOrgName,
    );
    if (org && org.projects.length > 0) {
      setSelectedProject(org.projects[0].name);
    }
  };

  const isAuthenticated = useIsAuthenticated();
  const { getAccessToken } = useApi();

  useEffect(() => {
    if (!isAuthenticated) return;

    async function fetchOrganizations() {
      try {
        setIsOrganizationsLoading(true);
        const token = await getAccessToken();
        const response = await fetch(`${apiUrl}/api/ado/organizations`, {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        const fetchedOrganizations: Organization[] = await response.json();
        setOrganizations(fetchedOrganizations);
        if (fetchedOrganizations.length > 0) {
          setSelectedOrganization(
            fetchedOrganizations[0].organization.accountName,
          );
          setSelectedProject(fetchedOrganizations[0].projects[0].name);
        }
      } catch (error) {
        console.log(error);
      } finally {
        setIsOrganizationsLoading(false);
      }
    }
    fetchOrganizations();
  }, [isAuthenticated, getAccessToken]);

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
        {isOrganizationsLoading ? (
          <p>Loading Organizations and Projects...</p>
        ) : (
          <>
            <select
              value={selectedOrganization}
              onChange={handleOrganizationSelection}
            >
              {organizations.map((org) => (
                <option
                  key={org.organization.accountId}
                  value={org.organization.accountName}
                >
                  {org.organization.accountName}
                </option>
              ))}
            </select>

            {(() => {
              const selectedOrg = organizations.find(
                (org) => org.organization.accountName === selectedOrganization,
              );
              return selectedOrg && selectedOrg.projects.length > 0 ? (
                <select
                  value={selectedProject}
                  onChange={handleProjectSelection}
                >
                  {selectedOrg.projects.map((project) => (
                    <option key={project.id} value={project.name}>
                      {project.name}
                    </option>
                  ))}
                </select>
              ) : (
                <p>No projects found</p>
              );
            })()}
          </>
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
          selectedOrganization={selectedOrganization}
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
