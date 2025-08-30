namespace Helsi.Todo.Domain.Entities;

public sealed class TaskList
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OwnerId { get; private set; }
    public string Title { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private TaskList() { }

    public TaskList(Guid ownerId, string title)
    {
        Rename(title);
        OwnerId = ownerId;
    }

    public void Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length is < 1 or > 255)
            throw new ArgumentException("Title length must be 1..255", nameof(title));
        Title = title.Trim();
    }
}