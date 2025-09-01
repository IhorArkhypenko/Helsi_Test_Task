using Helsi.Todo.Api.Dto;
using Helsi.Todo.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Helsi.Todo.Api.Controllers;

[ApiController]
[Route("tasklists")]
public class TaskListsController : ControllerBase
{
      private readonly ITaskListService _taskListService;

      public TaskListsController(ITaskListService taskListService)
      {
          _taskListService = taskListService;
      }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromHeader]Guid userId,
        [FromBody] CreateTaskListRequest request,
        CancellationToken ct)
    {
        var id = await _taskListService.CreateAsync(userId, request.Title, ct);
        return CreatedAtAction(nameof(GetById), new { taskListId = id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromHeader]Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20,
        CancellationToken ct = default)
    {
        var paged = await _taskListService.GetPagedAsync(userId, page, size, ct);

        var items = paged.Items
            .Select(x => new TaskListListItemDto(x.Id, x.Title))
            .ToList();

        return Ok(new PagedResponse<TaskListListItemDto>(items, paged.Total, paged.Page, paged.Size));
    }

    [HttpGet("{taskListId:guid}")]
    public async Task<IActionResult> GetById([FromHeader]Guid userId, Guid taskListId, CancellationToken ct)
    {
        var list = await _taskListService.GetByIdAsync(userId, taskListId, ct);
        if (list is null)
            return NotFound();

        return Ok(new TaskListDetailsDto(list.Id, list.Title, list.OwnerId, list.CreatedAtUtc));
    }

    [HttpPut("{taskListId:guid}")]
    public async Task<IActionResult> Rename(
        [FromHeader]Guid userId,
        Guid taskListId,
        [FromBody] RenameTaskListRequest request,
        CancellationToken ct)
    {
        await _taskListService.RenameAsync(userId, taskListId, request.Title, ct);
        return NoContent();
    }

    [HttpDelete("{taskListId:guid}")]
    public async Task<IActionResult> Delete(
        [FromHeader]Guid userId,
        Guid taskListId,
        CancellationToken ct)
    {
        await _taskListService.DeleteAsync(userId, taskListId, ct);
        return NoContent();
    }

    [HttpPost("{taskListId:guid}/users/{targetUserId:guid}")]
    public async Task<IActionResult> AddUser(
        [FromHeader]Guid userId,
        Guid taskListId,
        Guid targetUserId,
        CancellationToken ct)
    {
        await _taskListService.AddUserAsync(userId, taskListId, targetUserId, ct);
        return NoContent();
    }

    [HttpGet("{taskListId:guid}/users")]
    public async Task<IActionResult> GetUsers(
        [FromHeader]Guid userId,
        Guid taskListId,
        CancellationToken ct)
    {
        var userIds = await _taskListService.GetUsersAsync(userId, taskListId, ct);
        return Ok(new TaskListUsersDto(userIds));
    }

    [HttpDelete("{taskListId:guid}/users/{targetUserId:guid}")]
    public async Task<IActionResult> RemoveUser(
        [FromHeader]Guid userId,
        Guid taskListId,
        Guid targetUserId,
        CancellationToken ct)
    {
        await _taskListService.RemoveUserAsync(userId, taskListId, targetUserId, ct);
        return NoContent();
    }
}