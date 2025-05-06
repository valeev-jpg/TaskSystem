namespace TaskSystem.Domain.Models;

public class TaskHistory : Entity
{
    public Guid Id { get; set; }
    public Guid TaskTicketId { get; set; }
    public string Action { get; set; } = null!;
    public string By { get; set; } = null!;
    public DateTime At { get; set; }
    public TaskTicket TaskTicket { get; set; } = null!;
}