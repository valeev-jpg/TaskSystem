using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskSystem.Domain.Interfaces.Providers;
using TaskSystem.Domain.Models;
using TaskSystem.Models.TaskTicketModels;

namespace TaskSystem.Controllers;

[Authorize]
public class TaskTicketController(IMapper mapper, ITaskTicketCrudProvider crudProvider, IValidator<TaskTicket> validator, ITaskHistoryCrudProvider taskHistoryCrudProvider
) : BaseEntityController<TaskTicket>
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TaskTicketViewModel>>> GetAll()
    {
        var result = await crudProvider.GetAll();
        return mapper.Map<List<TaskTicketViewModel>>(result);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskTicketViewModel>> Get(Guid id)
    {
        var result = await crudProvider.Get(id);
        return mapper.Map<TaskTicketViewModel>(result);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Create(TaskTicketCreateModel objModel)
    {
        var obj = mapper.Map<TaskTicket>(objModel);
        
        await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await crudProvider.Create(obj);
        
        var by = User.FindFirstValue(ClaimTypes.Name) ?? "unknown";

        await taskHistoryCrudProvider.Create(new TaskHistory
        {
            TaskTicketId = obj.Id,
            Action = "Создание тикета",
            By = by,
            At = DateTime.UtcNow
        });
        
        return obj.Id;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Guid>>> BulkCreate(List<TaskTicketCreateModel> objModels)
    {
        var objects = mapper.Map<List<TaskTicket>>(objModels);
        
        foreach (var obj in objects)
            await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await crudProvider.CreateRange(objects);
        return objects.Select(obj => obj.Id).ToList();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Update(TaskTicketUpdateModel objModel)
    {
        var obj = mapper.Map<TaskTicket>(objModel);

        await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await crudProvider.Update(obj, 
            o => o.Title,
            o => o.Description,
            o => o.Due,
            o => o.Priority,
            o => o.Status,
            o => o.Assignee,
            o => o.Archived);

        var by = User.FindFirstValue(ClaimTypes.Name) ?? "unknown";

        await taskHistoryCrudProvider.Create(new TaskHistory
        {
            TaskTicketId = obj.Id,
            Action = "Обновление тикета",
            By = by,
            At = DateTime.UtcNow
        });
        
        return obj.Id;
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Delete(Guid id)
    {
        var result = await crudProvider.Delete(id);

        return result;
    }
}