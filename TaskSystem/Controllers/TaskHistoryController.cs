using System.Security.Claims;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using TaskSystem.Domain.Interfaces.Providers;
using TaskSystem.Domain.Models;
using TaskSystem.Models.TaskHistoryModels;
using TaskSystem.Models.TaskTicketModels;

namespace TaskSystem.Controllers;

[Authorize]
public class TaskHistoryController(IMapper mapper, ITaskHistoryCrudProvider crudProvider, IValidator<TaskHistory> validator
) : BaseEntityController<TaskHistory>
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TaskHistoryViewModel>>> GetAll()
    {
        var result = await crudProvider.GetAll();
        return mapper.Map<List<TaskHistoryViewModel>>(result);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TaskHistoryViewModel>> Get(Guid id)
    {
        var result = await crudProvider.Get(id);
        return mapper.Map<TaskHistoryViewModel>(result);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Create(TaskHistoryCreateModel objModel)
    {
        var obj = mapper.Map<TaskHistory>(objModel);
        
        await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        await crudProvider.Create(obj);
        
        return obj.Id;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Guid>>> BulkCreate(List<TaskHistoryCreateModel> objModels)
    {
        var objects = mapper.Map<List<TaskHistory>>(objModels);
        
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
    public async Task<ActionResult<Guid>> Update(TaskHistoryUpdateModel objModel)
    {
        var obj = mapper.Map<TaskHistory>(objModel);

        await ValidateAndChangeModelStateAsync(validator, obj, CancellationToken.None);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await crudProvider.Update(obj, 
            o => o.TaskTicketId,
            o => o.By,
            o => o.Action,
            o => o.At);

        return obj.Id;
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Delete(Guid id)
    {
        var result = await crudProvider.Delete(id);

        return result;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<TaskHistoryViewModel>>> ByTicket(Guid ticketId)
    {
        var result = await crudProvider.ByTicket(ticketId);

        return mapper.Map<List<TaskHistoryViewModel>>(result);
    }
}