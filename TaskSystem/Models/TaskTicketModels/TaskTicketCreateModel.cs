namespace TaskSystem.Models.TaskTicketModels;

public class TaskTicketCreateModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime Due { get; set; }
    public string Priority { get; set; } = "Medium";   // Low | Medium | High
    public string Status { get; set; } = "New";      // New | InProgress | Completed
    public string Assignee { get; set; } = "employee";
    public bool Archived { get; set; } = false;
}