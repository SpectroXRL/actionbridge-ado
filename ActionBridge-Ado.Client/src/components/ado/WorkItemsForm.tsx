import { useState } from "react";
import type { WorkItemRequest } from "../../types/WorkItemRequest";
import { useApi } from "../../utils/useApi";

const apiUrl = import.meta.env.VITE_API_URL;

interface WorkItemsFormProps {
  workItems: WorkItemRequest[];
  selectedOrganization: string;
  selectedProject: string;
  setWorkItems: React.Dispatch<React.SetStateAction<WorkItemRequest[]>>;
  setMessage: (message: string | null) => void;
  setIsError: (isError: boolean) => void;
}

const workItemTypes = ["Task", "Epic", "Issue"] as const;
const priorities = [
  { value: 1, label: "1 - Critical" },
  { value: 2, label: "2 - High" },
  { value: 3, label: "3 - Medium" },
  { value: 4, label: "4 - Low" },
];

const WorkItemsForm = ({
  workItems,
  selectedOrganization,
  selectedProject,
  setWorkItems,
  setMessage,
  setIsError,
}: WorkItemsFormProps) => {
  const [isSubmitting, setIsSubmitting] = useState(false);

  const { getAccessToken } = useApi();

  const handleCreateWorkItems = async () => {
    setIsSubmitting(true);
    setMessage(null);
    setIsError(false);

    try {
      const token = await getAccessToken();

      const response = await fetch(
        `${apiUrl}/api/ado/workitems?organizationUrl=${encodeURIComponent(selectedOrganization)}&project=${encodeURIComponent(selectedProject)}`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
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
        `Successfully created ${result.count} work items in Azure DevOps!`,
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

  const updateWorkItem = (
    index: number,
    field: keyof WorkItemRequest,
    value: string | number | null,
  ) => {
    setWorkItems((prev) =>
      prev.map((item, i) => (i === index ? { ...item, [field]: value } : item)),
    );
  };

  const removeWorkItem = (index: number) => {
    setWorkItems((prev) => prev.filter((_, i) => i !== index));
  };

  const addWorkItem = () => {
    setWorkItems((prev) => [
      ...prev,
      {
        title: "",
        description: "",
        type: "Task",
        tags: null,
        assignedTo: null,
        priority: 3,
      },
    ]);
  };

  const renderTags = (tags: string | null) => {
    if (!tags) return null;
    const tagList = tags
      .split(",")
      .map((t) => t.trim())
      .filter(Boolean);
    if (tagList.length === 0) return null;

    return (
      <div className="tags-preview">
        {tagList.map((tag, i) => (
          <span key={i} className="tag">
            {tag}
          </span>
        ))}
      </div>
    );
  };

  if (workItems.length === 0) {
    return null;
  }

  return (
    <div className="work-items-form">
      <h2>Review Work Items ({workItems.length})</h2>

      {workItems.map((item, index) => (
        <div key={index} className="work-item-card" data-type={item.type}>
          <div className="work-item-header">
            <span className="work-item-number">#{index + 1}</span>
            <span className={`priority-${item.priority}`}>
              Priority:{" "}
              {priorities.find((p) => p.value === item.priority)?.label}
            </span>
            <button
              type="button"
              onClick={() => removeWorkItem(index)}
              className="remove-btn"
            >
              Remove
            </button>
          </div>

          <div className="form-group">
            <label>Title</label>
            <input
              type="text"
              value={item.title}
              onChange={(e) => updateWorkItem(index, "title", e.target.value)}
              required
            />
          </div>

          <div className="form-group">
            <label>Description</label>
            <textarea
              value={item.description}
              onChange={(e) =>
                updateWorkItem(index, "description", e.target.value)
              }
              rows={3}
            />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Type</label>
              <select
                value={item.type}
                onChange={(e) => updateWorkItem(index, "type", e.target.value)}
              >
                {workItemTypes.map((type) => (
                  <option key={type} value={type}>
                    {type}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label>Priority</label>
              <select
                value={item.priority}
                onChange={(e) =>
                  updateWorkItem(index, "priority", parseInt(e.target.value))
                }
              >
                {priorities.map((p) => (
                  <option key={p.value} value={p.value}>
                    {p.label}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="form-group">
            <label>Tags (comma-separated)</label>
            <input
              type="text"
              value={item.tags ?? ""}
              onChange={(e) =>
                updateWorkItem(index, "tags", e.target.value || null)
              }
            />
            {renderTags(item.tags)}
          </div>

          <div className="form-group">
            <label>Assigned To</label>
            <input
              type="text"
              value={item.assignedTo ?? ""}
              onChange={(e) =>
                updateWorkItem(index, "assignedTo", e.target.value || null)
              }
              placeholder="email@example.com"
            />
          </div>
        </div>
      ))}

      <div className="form-actions">
        <button type="button" onClick={addWorkItem} className="add-btn">
          + Add Work Item
        </button>
        <button
          type="button"
          onClick={handleCreateWorkItems}
          disabled={isSubmitting}
          className="submit-btn"
        >
          {isSubmitting ? "Creating..." : "Create Work Items in Azure DevOps"}
        </button>
      </div>
    </div>
  );
};

export default WorkItemsForm;
