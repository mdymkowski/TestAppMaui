using FluentValidation;
using TestAppMaui.MauiClient.Domain.Tasks;

namespace TestAppMaui.MauiClient.Application.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage("Minimum 3 znaki");

        RuleFor(command => command.Description)
            .MaximumLength(TaskItem.DescriptionMaxLength)
            .WithMessage($"Maksymalnie {TaskItem.DescriptionMaxLength} znaków");
    }
}
