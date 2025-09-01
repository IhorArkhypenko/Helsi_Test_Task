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

      /// <summary>
      /// Create a new task list.
      /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromHeader]Guid userId,
        [FromBody] CreateTaskListRequest request,
        CancellationToken ct)
    {
        var id = await _taskListService.CreateAsync(userId, request.Title, ct);
        return CreatedAtAction(nameof(GetById), new { taskListId = id }, new { id });
    }

    /// <summary>
    /// Get all task lists for the user (with pagination).
    /// </summary>
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

    /// <summary>
    /// Get task list details by ID.
    /// </summary>
    [HttpGet("{taskListId:guid}")]
    public async Task<IActionResult> GetById([FromHeader]Guid userId, Guid taskListId, CancellationToken ct)
    {
        var list = await _taskListService.GetByIdAsync(userId, taskListId, ct);
        if (list is null)
            return NotFound();

        return Ok(new TaskListDetailsDto(list.Id, list.Title, list.OwnerId, list.CreatedAtUtc));
    }

    /// <summary>
    /// Rename an existing task list.
    /// </summary>
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

    /// <summary>
    /// Delete a task list by ID.
    /// </summary>
    [HttpDelete("{taskListId:guid}")]
    public async Task<IActionResult> Delete(
        [FromHeader]Guid userId,
        Guid taskListId,
        CancellationToken ct)
    {
        await _taskListService.DeleteAsync(userId, taskListId, ct);
        return NoContent();
    }

    /// <summary>
    /// Share task list with another user.
    /// </summary>
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

    /// <summary>
    /// Get all users assigned to the task list.
    /// </summary>
    [HttpGet("{taskListId:guid}/users")]
    public async Task<IActionResult> GetUsers(
        [FromHeader]Guid userId,
        Guid taskListId,
        CancellationToken ct)
    {
        var userIds = await _taskListService.GetUsersAsync(userId, taskListId, ct);
        return Ok(new TaskListUsersDto(userIds));
    }

    /// <summary>
    /// Remove a user from the task list.
    /// </summary>
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