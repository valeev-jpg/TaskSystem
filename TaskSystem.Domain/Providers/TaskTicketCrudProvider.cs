using Microsoft.EntityFrameworkCore;
using TaskSystem.Domain.Exceptions;
using TaskSystem.Domain.Interfaces;
using TaskSystem.Domain.Interfaces.Providers;
using TaskSystem.Domain.Models;

namespace TaskSystem.Domain.Providers;

public class TaskTicketCrudProvider(IServiceDbContext context) : BaseCrudProvider<TaskTicket>(context), ITaskTicketCrudProvider
{
    
}