using Helsi.Todo.Application.Abstractions;
using Helsi.Todo.Infrastructure.Persistence;
using Helsi.Todo.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helsi.Todo.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<TodoDbContext>(o =>
            o.UseNpgsql(cfg.GetConnectionString("Postgres")));

        services.AddScoped<ITaskListRepository, TaskListRepository>();

        return services;
    }
}