using Helsi.Todo.Application.Common;
using Helsi.Todo.Domain.Entities;

namespace Helsi.Todo.Application.Abstractions;

public interface ITaskListRepository
{
    Task<TaskList?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(TaskList entity, CancellationToken ct);
    void Update(TaskList entity);
    void Remove(TaskList entity);

    Task<bool> IsUserLinkedAsync(Guid listId, Guid userId, CancellationToken ct);
    Task AddUserLinkAsync(Guid listId, Guid userId, CancellationToken ct);
    Task RemoveUserLinkAsync(Guid listId, Guid userId, CancellationToken ct);
    Task<IReadOnlyList<Guid>> GetLinkedUsersAsync(Guid listId, CancellationToken ct);

    Task<PagedResult<TaskList>> GetForUserAsync(Guid userId, int page, int size, CancellationToken ct);
}