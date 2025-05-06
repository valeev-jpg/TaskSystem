using FluentValidation;
using TaskSystem.Domain.Models;

namespace TaskSystem.Domain.Validation;

public class TaskTicketValidator : AbstractValidator<TaskTicket>
{
    public TaskTicketValidator()
    {
    }
}