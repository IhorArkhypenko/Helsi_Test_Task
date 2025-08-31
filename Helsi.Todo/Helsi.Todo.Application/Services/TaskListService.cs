using Helsi.Todo.Application.Abstractions;
using Helsi.Todo.Application.Common;
using Helsi.Todo.Domain.Entities;

namespace Helsi.Todo.Application.Services;

public class TaskListService : ITaskListService
{
    private readonly ITaskListRepository _taskListRepo;
    private readonly TaskListAccessPolicy _taskListAccessPolicy;

    public TaskListService(ITaskListRepository taskListRepo, TaskListAccessPolicy taskListAccessPolicy)
    {
        _taskListRepo = taskListRepo;
        _taskListAccessPolicy = taskListAccessPolicy;
    }

    public async Task<Guid> CreateAsync(Guid userId, string title, CancellationToken ct)
    {
        var list = new TaskList(userId, title);

        await _taskListRepo.AddAsync(list, ct);
        await _taskListRepo.SaveChangesAsync(ct);

        return list.Id;
    }

    public async Task<TaskList?> GetByIdAsync(Guid userId, Guid listId, CancellationToken ct)
    {
        var list = await _taskListRepo.GetByIdAsync(listId, ct);
        if (list is null)
            return null;

        var canRead = await _taskListAccessPolicy.CanUpdateAsync(list, userId, ct) || list.OwnerId == userId;
        if (!canRead)
            throw new UnauthorizedAccessException();

        return list;
    }

    public Task<PagedResult<TaskList>> GetPagedAsync(Guid userId, int page, int size, CancellationToken ct)
        => _taskListRepo.GetForUserAsync(userId, page, size, ct);

    public async Task RenameAsync(Guid userId, Guid listId, string title, CancellationToken ct)
    {
        var list = await _taskListRepo.GetByIdAsync(listId, ct) ?? throw new KeyNotFoundException();

        if (!await _taskListAccessPolicy.CanUpdateAsync(list, userId, ct))
            throw new UnauthorizedAccessException();

        list.Rename(title);
        _taskListRepo.Update(list);
        await _taskListRepo.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid userId, Guid listId, CancellationToken ct)
    {
        var list = await _taskListRepo.GetByIdAsync(listId, ct) ?? throw new KeyNotFoundException();

        if (!await _taskListAccessPolicy.CanDeleteAsync(list, userId, ct))
            throw new UnauthorizedAccessException();

        _taskListRepo.Remove(list);
        await _taskListRepo.SaveChangesAsync(ct);
    }

    public async Task AddUserAsync(Guid userId, Guid listId, Guid targetUserId, CancellationToken ct)
    {
        var list = await _taskListRepo.GetByIdAsync(listId, ct) ?? throw new KeyNotFoundException();

        if (!await _taskListAccessPolicy.CanManageUsersAsync(list, userId, ct))
            throw new UnauthorizedAccessException();

        if (targetUserId == list.OwnerId)
            throw new InvalidOperationException("Owner already has access");

        if (await _taskListRepo.IsUserLinkedAsync(list.Id, targetUserId, ct))
            throw new InvalidOperationException("User already linked");

        await _taskListRepo.AddUserLinkAsync(list.Id, targetUserId, ct);
        await _taskListRepo.SaveChangesAsync(ct);
    }

    public async Task RemoveUserAsync(Guid userId, Guid listId, Guid targetUserId, CancellationToken ct)
    {
        var list = await _taskListRepo.GetByIdAsync(listId, ct) ?? throw new KeyNotFoundException();

        if (!await _taskListAccessPolicy.CanManageUsersAsync(list, userId, ct))
            throw new UnauthorizedAccessException();

        if (targetUserId == list.OwnerId)
            throw new InvalidOperationException("Cannot remove owner");

        await _taskListRepo.RemoveUserLinkAsync(list.Id, targetUserId, ct);
        await _taskListRepo.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Guid>> GetUsersAsync(Guid userId, Guid listId, CancellationToken ct)
    {
        var list = await _taskListRepo.GetByIdAsync(listId, ct) ?? throw new KeyNotFoundException();

        if (!await _taskListAccessPolicy.CanManageUsersAsync(list, userId, ct))
            throw new UnauthorizedAccessException();

        return await _taskListRepo.GetLinkedUsersAsync(list.Id, ct);
    }
}