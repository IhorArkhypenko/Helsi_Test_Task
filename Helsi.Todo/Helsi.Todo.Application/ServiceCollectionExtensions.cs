using Helsi.Todo.Application.Abstractions;
using Helsi.Todo.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Helsi.Todo.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TaskListAccessPolicy>();
        services.AddScoped<ITaskListService, TaskListService>();
        return services;
    }
}