using TaskSystem.Domain.Models;

namespace TaskSystem.Domain.Interfaces.Providers;

public interface ITaskHistoryCrudProvider : ICrudProvider<TaskHistory>
{
    public Task<List<TaskHistory>> ByTicket(Guid ticketId);
}