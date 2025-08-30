namespace Helsi.Todo.Application.UseCases.TaskLists.Create;

public sealed record CreateTaskListCommand(Guid UserId, string Title);