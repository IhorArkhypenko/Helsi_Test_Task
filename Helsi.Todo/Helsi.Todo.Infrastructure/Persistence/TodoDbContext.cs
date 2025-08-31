using Helsi.Todo.Domain.Entities;
using Helsi.Todo.Infrastructure.Persistence.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Helsi.Todo.Infrastructure.Persistence;

public sealed class TodoDbContext : DbContext
{
    public DbSet<TaskList> TaskLists => Set<TaskList>();
    public DbSet<TaskListUser> TaskListUsers => Set<TaskListUser>();

    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new TaskListConfiguration());
        builder.ApplyConfiguration(new TaskListUserConfiguration());
    }
}