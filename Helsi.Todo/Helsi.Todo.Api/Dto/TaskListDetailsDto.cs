namespace Helsi.Todo.Api.Dto;

public sealed record TaskListDetailsDto(Guid Id, string Title, Guid OwnerId, DateTime CreatedAtUtc);