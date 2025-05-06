export type Priority = "Low" | "Medium" | "High";
export type Status   = "New" | "InProgress" | "Completed";

export interface TaskHistory {
  id: string;
  taskTicketId: string;
  action: string;
  by: string;
  at: string;               // ISO
}

export interface TaskTicket {
  id: string;               // Guid приходит как строка
  title: string;
  description: string;
  priority: Priority;
  status: Status;
  due: string;              // ISO
  assignee: string;
  archived: boolean;
  history?: TaskHistory[];
}
