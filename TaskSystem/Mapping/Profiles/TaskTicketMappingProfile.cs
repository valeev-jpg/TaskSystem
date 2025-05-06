using AutoMapper;
using TaskSystem.Domain.Models;
using TaskSystem.Models.TaskTicketModels;

namespace TaskSystem.Mapping.Profiles;

public class TaskTicketMappingProfile : Profile
{
    public TaskTicketMappingProfile()
    {
        CreateMap<TaskTicket, TaskTicketViewModel>();
        CreateMap<TaskTicketCreateModel, TaskTicket>();
        CreateMap<TaskTicketUpdateModel, TaskTicket>();
    }
}