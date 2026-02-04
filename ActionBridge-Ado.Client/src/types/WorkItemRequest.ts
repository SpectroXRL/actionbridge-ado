export interface WorkItemRequest {
  title: string;
  description: string;
  type: "Task" | "Epic" | "Issue";
  tags: string | null;
  assignedTo: string | null;
  priority: 1 | 2 | 3 | 4;
}