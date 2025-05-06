using FluentValidation;
using TaskSystem.Domain.Models;

namespace TaskSystem.Domain.Validation;

public class TaskHistoryValidator : AbstractValidator<TaskHistory>
{
    public TaskHistoryValidator()
    {
    }
}