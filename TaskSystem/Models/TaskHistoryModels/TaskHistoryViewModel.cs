namespace TaskSystem.Models.TaskHistoryModels;

public class TaskHistoryViewModel
{
    public Guid Id { get; set; }
    public Guid TaskTicketId { get; set; }
    public string Action { get; set; } = null!;
    public string By { get; set; } = null!;
    public DateTime At { get; set; }
}