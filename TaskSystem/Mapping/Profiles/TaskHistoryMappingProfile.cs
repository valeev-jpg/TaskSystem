using AutoMapper;
using TaskSystem.Domain.Models;
using TaskSystem.Models.TaskHistoryModels;
using TaskSystem.Models.TaskTicketModels;

namespace TaskSystem.Mapping.Profiles;

public class TaskTicketsMappingProfile : Profile
{
    public TaskTicketsMappingProfile()
    {
        CreateMap<TaskHistory, TaskHistoryViewModel>();
        CreateMap<TaskHistoryCreateModel, TaskHistory>();
        CreateMap<TaskHistoryUpdateModel, TaskHistory>();
    }
}