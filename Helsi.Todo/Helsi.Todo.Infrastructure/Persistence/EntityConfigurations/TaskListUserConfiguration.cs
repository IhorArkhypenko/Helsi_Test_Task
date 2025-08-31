using Helsi.Todo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Helsi.Todo.Infrastructure.Persistence.EntityConfigurations;

public class TaskListUserConfiguration : IEntityTypeConfiguration<TaskListUser>
{
    public void Configure(EntityTypeBuilder<TaskListUser> builder)
    {
        builder.ToTable("task_list_users");
        builder.HasKey(x => new { x.TaskListId, x.UserId });
        builder.HasIndex(x => new { x.TaskListId, x.UserId }).IsUnique();
    }
}