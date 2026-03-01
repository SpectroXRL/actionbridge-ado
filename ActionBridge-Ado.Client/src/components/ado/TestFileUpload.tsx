import { useState, type FormEvent } from "react";
import type { WorkItemRequest } from "../../types/WorkItemRequest";

interface TestFileUploadProps {
  setMessage: (message: string | null) => void;
  setIsError: (isError: boolean) => void;
  setWorkItems: (workItems: WorkItemRequest[]) => void;
}

const TestFileUpload = ({
  setMessage,
  setIsError,
  setWorkItems,
}: TestFileUploadProps) => {
  const [isUploading, setIsUploading] = useState(false);

  const handleTestFileUpload = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsUploading(true);
    setMessage(null);
    setIsError(false);

    try {
      const result: WorkItemRequest[] = [
        {
          title: "Set up project repository",
          description:
            "Initialize Git repository with proper branching strategy and CI/CD pipeline configuration.",
          type: "Task",
          tags: "setup;infrastructure",
          assignedTo: null,
          priority: 1,
        },
        {
          title: "User Authentication Epic",
          description:
            "Implement complete user authentication flow including login, logout, and session management.",
          type: "Epic",
          tags: "auth;security",
          assignedTo: null,
          priority: 2,
        },
        {
          title: "Fix login redirect issue",
          description:
            "Users are not being redirected to the dashboard after successful login. Investigate and resolve.",
          type: "Issue",
          tags: "bug;auth",
          assignedTo: null,
          priority: 1,
        },
        {
          title: "Create API documentation",
          description:
            "Document all REST API endpoints with request/response examples using OpenAPI specification.",
          type: "Task",
          tags: "documentation;api",
          assignedTo: null,
          priority: 3,
        },
        {
          title: "Performance optimization",
          description:
            "Analyze and optimize database queries causing slow page load times on the dashboard.",
          type: "Task",
          tags: "performance;database",
          assignedTo: null,
          priority: 2,
        },
      ];

      await new Promise((resolve) => setTimeout(resolve, 500));

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

  return (
    <>
      <form onSubmit={handleTestFileUpload} className="upload-form">
        <button type="submit" disabled={isUploading}>
          {isUploading ? "Processing..." : "Upload & Generate Test Work Items"}
        </button>
      </form>
    </>
  );
};

export default TestFileUpload;
