import { useState, type FormEvent } from "react";
import WorkItemsForm from "./components/WorkItemsForm";
import type { WorkItemRequest } from "./types/WorkItemRequest";
import "./App.css";

const App = () => {
  const [workItems, setWorkItems] = useState<WorkItemRequest[]>([]);
  const [isUploading, setIsUploading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [isError, setIsError] = useState(false);

  const organization = import.meta.env.VITE_ORGANIZATION;
  const project = import.meta.env.VITE_PROJECT;
  const organizationUrl = import.meta.env.VITE_ORGANIZATION_URL;

  const handleFileUpload = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsUploading(true);
    setMessage(null);
    setIsError(false);

    const form = e.currentTarget;
    const formData = new FormData(form);

    try {
      const response = await fetch(
        `http://localhost:5277/api/file/upload?organization=${encodeURIComponent(organization)}&project=${encodeURIComponent(project)}`,
        {
          method: "POST",
          body: formData,
        },
      );

      if (!response.ok) {
        throw new Error("Failed to upload file");
      }

      const result: WorkItemRequest[] = await response.json();
      setWorkItems(result);
      setMessage(
        `Generated ${result.length} work items. Review and edit below.`,
      );
      setIsError(false);
    } catch (error) {
      setMessage(
        `Error: ${error instanceof Error ? error.message : "Unknown error"}`,
      );
      setIsError(true);
    } finally {
      setIsUploading(false);
    }
  };

  const handleCreateWorkItems = async () => {
    setIsSubmitting(true);
    setMessage(null);
    setIsError(false);

    try {
      const response = await fetch(
        `http://localhost:5277/api/ado/workitems?organizationUrl=${encodeURIComponent(organizationUrl)}&project=${encodeURIComponent(project)}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(workItems),
        },
      );

      if (!response.ok) {
        throw new Error("Failed to create work items");
      }

      const result = await response.json();
      setMessage(
        `Successfully created ${result.created} work items in Azure DevOps!`,
      );
      setIsError(false);
      setWorkItems([]);
    } catch (error) {
      setMessage(
        `Error: ${error instanceof Error ? error.message : "Unknown error"}`,
      );
      setIsError(true);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="app-container">
      <h1>ActionBridge - Azure DevOps Work Item Generator</h1>

      <form onSubmit={handleFileUpload} className="upload-form">
        <input
          type="file"
          id="txtfile"
          name="file"
          accept=".txt,.doc,.docx,.xml,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        />
        <button type="submit" disabled={isUploading}>
          {isUploading ? "Processing..." : "Upload & Generate Work Items"}
        </button>
      </form>

      {message && (
        <div className={`message ${isError ? "error" : "success"}`}>
          {message}
        </div>
      )}

      <WorkItemsForm
        workItems={workItems}
        setWorkItems={setWorkItems}
        onSubmit={handleCreateWorkItems}
        isSubmitting={isSubmitting}
      />
    </div>
  );
};

export default App;
