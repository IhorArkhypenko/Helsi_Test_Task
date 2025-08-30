namespace Helsi.Todo.Domain.Entities;

public sealed class TaskListUser
{
    public Guid TaskListId { get; set; }
    public Guid UserId { get; set; }
}