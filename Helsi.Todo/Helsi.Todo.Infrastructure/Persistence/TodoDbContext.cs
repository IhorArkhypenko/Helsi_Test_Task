using Helsi.Todo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Helsi.Todo.Infrastructure.Persistence;

public sealed class TodoDbContext : DbContext
{
    public DbSet<TaskList> TaskLists => Set<TaskList>();
    public DbSet<TaskListUser> TaskListUsers => Set<TaskListUser>();

    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // I prefer to move configurations to separate files in order to avoid big DbContext file, but in this case it's redundant.
        builder.Entity<TaskList>(e =>
        {
            e.ToTable("task_lists");
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(255).IsRequired();
            e.Property(x => x.OwnerId).IsRequired();
            e.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc");
            e.HasIndex(x => x.OwnerId);
            e.HasIndex(x => x.CreatedAtUtc);
        });

        builder.Entity<TaskListUser>(e =>
        {
            e.ToTable("task_list_users");
            e.HasKey(x => new { x.TaskListId, x.UserId });
            e.HasIndex(x => new { x.TaskListId, x.UserId }).IsUnique();
        });
    }
}