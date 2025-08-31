using FluentValidation;
using Helsi.Todo.Api.Dto;

namespace Helsi.Todo.Api.Validation.TaskLists;

public sealed class CreateTaskListRequestValidator : AbstractValidator<CreateTaskListRequest>
{
    public CreateTaskListRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(255).WithMessage("Title must be <= 255 characters");
    }
}