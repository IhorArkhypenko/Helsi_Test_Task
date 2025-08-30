namespace Helsi.Todo.Api.Contracts;

public sealed record TaskListDetailsDto(Guid Id, string Title, Guid OwnerId, DateTime CreatedAtUtc);