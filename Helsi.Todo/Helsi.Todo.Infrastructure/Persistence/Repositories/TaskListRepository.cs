using Helsi.Todo.Application.Abstractions;
using Helsi.Todo.Application.Common;
using Helsi.Todo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helsi.Todo.Infrastructure.Persistence.Repositories;

public class TaskListRepository : ITaskListRepository
{
    private readonly TodoDbContext _dbContext;

    public TaskListRepository(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<TaskList?> GetByIdAsync(Guid id, CancellationToken ct)
        => _dbContext.TaskLists.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<PagedResult<TaskList>> GetForUserAsync(Guid userId, int page, int size, CancellationToken ct)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 20;
        if (size > 100) size = 100;

        var taskListsQuery = _dbContext.TaskLists
            .Where(tl => tl.OwnerId == userId
                         || _dbContext.TaskListUsers.Any(l => l.TaskListId == tl.Id && l.UserId == userId))
            .OrderByDescending(tl => tl.CreatedAtUtc);

        var total = await taskListsQuery.CountAsync(ct);
        var items = await taskListsQuery.Skip((page - 1) * size).Take(size).ToListAsync(ct);

        return new PagedResult<TaskList>(items, total, page, size);
    }

    public Task AddAsync(TaskList entity, CancellationToken ct)
        => _dbContext.TaskLists.AddAsync(entity, ct).AsTask();

    public void Update(TaskList entity) => _dbContext.TaskLists.Update(entity);
    public void Remove(TaskList entity) => _dbContext.TaskLists.Remove(entity);

    public Task<bool> IsUserLinkedAsync(Guid listId, Guid userId, CancellationToken ct)
        => _dbContext.TaskListUsers.AnyAsync(x => x.TaskListId == listId && x.UserId == userId, ct);

    public Task AddUserLinkAsync(Guid listId, Guid userId, CancellationToken ct)
    {
        _dbContext.TaskListUsers.Add(new TaskListUser { TaskListId = listId, UserId = userId });
        return Task.CompletedTask;
    }

    public async Task RemoveUserLinkAsync(Guid listId, Guid userId, CancellationToken ct)
    {
        var entity = await _dbContext.TaskListUsers.FindAsync([listId, userId], ct);
        if (entity is not null) _dbContext.TaskListUsers.Remove(entity);
    }

    public async Task<IReadOnlyList<Guid>> GetLinkedUsersAsync(Guid listId, CancellationToken ct)
        => await _dbContext.TaskListUsers.Where(x => x.TaskListId == listId).Select(x => x.UserId).ToListAsync(ct);

    public Task<int> SaveChangesAsync(CancellationToken ct) => _dbContext.SaveChangesAsync(ct);
}