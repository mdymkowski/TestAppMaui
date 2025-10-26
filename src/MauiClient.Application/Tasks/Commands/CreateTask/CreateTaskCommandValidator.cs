using FluentValidation;

namespace TestAppMaui.MauiClient.Application.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage("Minimum 3 znaki");
    }
}
