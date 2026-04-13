import { useState, type FormEvent } from "react";
import type { WorkItemRequest } from "../../types/WorkItemRequest";
import { useApi } from "../../utils/useApi";

const apiUrl = import.meta.env.VITE_API_URL;

interface TranscriptFileUploadProps {
  setMessage: (message: string | null) => void;
  setIsError: (isError: boolean) => void;
  setWorkItems: (workItems: WorkItemRequest[]) => void;
}

const TranscriptFileUpload = ({
  setMessage,
  setIsError,
  setWorkItems,
}: TranscriptFileUploadProps) => {
  const [isUploading, setIsUploading] = useState(false);

  const { getAccessToken } = useApi();

  const handleFileUpload = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setIsUploading(true);
    setMessage(null);
    setIsError(false);

    const form = e.currentTarget;
    const formData = new FormData(form);

    try {
      const token = await getAccessToken();

      const response = await fetch(`${apiUrl}/api/file/upload`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
        body: formData,
      });

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

  return (
    <>
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
    </>
  );
};

export default TranscriptFileUpload;
