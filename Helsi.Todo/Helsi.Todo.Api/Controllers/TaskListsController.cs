using Helsi.Todo.Api.Dto;
using Helsi.Todo.Api.Extensions;
using Helsi.Todo.Application.Abstractions;
using Helsi.Todo.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Helsi.Todo.Api.Controllers;

[ApiController]
[Route("tasklists")]
public class TaskListsController : ControllerBase
{
     private readonly ITaskListRepository _taskListRepo;

     public TaskListsController(ITaskListRepository taskListRepo)
     {
         _taskListRepo = taskListRepo;
     }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskListRequest request, CancellationToken ct)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        try
        {
            var taskList = new TaskList(userId, request.Title);
            await _taskListRepo.AddAsync(taskList, ct);
            await _taskListRepo.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(GetById), new { id = taskList.Id });
        }
        catch (ArgumentException ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        var pagedTaskLists = await _taskListRepo.GetForUserAsync(userId, page, size, ct);

        var listItemDtos = pagedTaskLists.Items
            .Select(x => new TaskListListItemDto(x.Id, x.Title))
            .ToList();

        var response = new PagedResponse<TaskListListItemDto>(
            listItemDtos, pagedTaskLists.Total, pagedTaskLists.Page, pagedTaskLists.Size);

        return Ok(response);
    }

    [HttpGet("{taskListId:guid}")]
    public async Task<IActionResult> GetById(Guid taskListId, CancellationToken ct)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        var taskList = await _taskListRepo.GetByIdAsync(taskListId, ct);
        if (taskList is null)
            return NotFound();

        if (taskList.OwnerId != userId && !await _taskListRepo.IsUserLinkedAsync(taskList.Id, userId, ct))
            return Forbid();

        var taskListDetailsDto =
            new TaskListDetailsDto(taskList.Id, taskList.Title, taskList.OwnerId, taskList.CreatedAtUtc);

        return Ok(taskListDetailsDto);
    }
}