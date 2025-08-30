using Helsi.Todo.Application.Abstractions;
using Helsi.Todo.Domain.Entities;

namespace Helsi.Todo.Application.UseCases.TaskLists.Create;

public class CreateTaskListHandler
{
    private readonly ITaskListRepository _taskListRepo;
    private readonly IUnitOfWork _uow;

    public CreateTaskListHandler(ITaskListRepository taskListRepo, IUnitOfWork uow)
    {
        _taskListRepo = taskListRepo;
        _uow = uow;
    }

    public async Task<Guid> HandleAsync(CreateTaskListCommand cmd, CancellationToken ct)
    {
        var list = new TaskList(cmd.UserId, cmd.Title);

        await _taskListRepo.AddAsync(list, ct);
        await _uow.SaveChangesAsync(ct);

        return list.Id;
    }
}