namespace Helsi.Todo.Api.Dto;

public sealed record TaskListUsersDto(IReadOnlyList<Guid> UserIds);