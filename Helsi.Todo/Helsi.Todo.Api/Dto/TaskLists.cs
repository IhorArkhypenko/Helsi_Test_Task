namespace Helsi.Todo.Api.Dto;

public sealed record TaskListListItemDto(Guid Id, string Title);
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Total, int Page, int Size);