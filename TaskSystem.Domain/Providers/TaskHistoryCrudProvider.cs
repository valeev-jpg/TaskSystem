using Microsoft.EntityFrameworkCore;
using TaskSystem.Domain.Interfaces;
using TaskSystem.Domain.Interfaces.Providers;
using TaskSystem.Domain.Models;

namespace TaskSystem.Domain.Providers;

public class TaskHistoryCrudProvider(IServiceDbContext context) : BaseCrudProvider<TaskHistory>(context), ITaskHistoryCrudProvider
{
    public async Task<List<TaskHistory>> ByTicket(Guid ticketId)
    {
        var history = await context.TaskHistories
            .Where(h => h.TaskTicketId == ticketId)
            .OrderByDescending(h => h.At)
            .ToListAsync();

        return history;
    }
}