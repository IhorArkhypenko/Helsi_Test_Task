namespace Helsi.Todo.Api.Dto;

public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Total, int Page, int Size);