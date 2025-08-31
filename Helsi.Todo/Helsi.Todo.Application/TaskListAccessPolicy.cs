using Helsi.Todo.Application.Abstractions;
using Helsi.Todo.Domain.Entities;

namespace Helsi.Todo.Application;

public sealed class TaskListAccessPolicy
{
    private readonly ITaskListRepository _taskListRepo;

    public TaskListAccessPolicy(ITaskListRepository taskListRepo)
    {
        _taskListRepo = taskListRepo;
    }

    public Task<bool> CanReadAsync(TaskList list, Guid userId, CancellationToken ct)
        => Task.FromResult(list.OwnerId == userId);

    public async Task<bool> CanUpdateAsync(TaskList list, Guid userId, CancellationToken ct)
    {
        return list.OwnerId == userId || await _taskListRepo.IsUserLinkedAsync(list.Id, userId, ct);
    }

    public Task<bool> CanDeleteAsync(TaskList list, Guid userId, CancellationToken ct)
        => Task.FromResult(list.OwnerId == userId);

    public async Task<bool> CanManageUsersAsync(TaskList list, Guid userId, CancellationToken ct)
    {
        return list.OwnerId == userId || await _taskListRepo.IsUserLinkedAsync(list.Id, userId, ct);
    }
}