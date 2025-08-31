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
      private readonly ITaskListService _taskListService;

      public TaskListsController(ITaskListService taskListService)
      {
          _taskListService = taskListService;
      }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskListRequest request, CancellationToken ct)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        var id = await _taskListService.CreateAsync(userId, request.Title, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        var paged = await _taskListService.GetPagedAsync(userId, page, size, ct);

        var items = paged.Items
            .Select(x => new TaskListListItemDto(x.Id, x.Title))
            .ToList();

        return Ok(new PagedResponse<TaskListListItemDto>(items, paged.Total, paged.Page, paged.Size));
    }

    [HttpGet("{taskListId:guid}")]
    public async Task<IActionResult> GetById(Guid taskListId, CancellationToken ct)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        var list = await _taskListService.GetByIdAsync(userId, taskListId, ct);
        if (list is null)
            return NotFound();

        return Ok(new TaskListDetailsDto(list.Id, list.Title, list.OwnerId, list.CreatedAtUtc));
    }

    [HttpPut("{taskListId:guid}")]
    public async Task<IActionResult> Rename(Guid taskListId, [FromBody] RenameTaskListRequest request, CancellationToken ct)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        await _taskListService.RenameAsync(userId, taskListId, request.Title, ct);
        return NoContent();
    }

    [HttpDelete("{taskListId:guid}")]
    public async Task<IActionResult> Delete(Guid taskListId, CancellationToken ct)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        await _taskListService.DeleteAsync(userId, taskListId, ct);
        return NoContent();
    }

    [HttpPost("{taskListId:guid}/users/{targetUserId:guid}")]
    public async Task<IActionResult> AddUser(Guid taskListId, Guid targetUserId, CancellationToken ct)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        await _taskListService.AddUserAsync(userId, taskListId, targetUserId, ct);
        return NoContent();
    }

    [HttpGet("{taskListId:guid}/users")]
    public async Task<IActionResult> GetUsers(Guid taskListId, CancellationToken ct)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        var userIds = await _taskListService.GetUsersAsync(userId, taskListId, ct);
        return Ok(new TaskListUsersDto(userIds));
    }

    [HttpDelete("{taskListId:guid}/users/{targetUserId:guid}")]
    public async Task<IActionResult> RemoveUser(Guid taskListId, Guid targetUserId, CancellationToken ct)
    {
        if (!HttpContext.TryGetUserId(out var userId))
            return BadRequest("X-User-Id header is required");

        await _taskListService.RemoveUserAsync(userId, taskListId, targetUserId, ct);
        return NoContent();
    }
}