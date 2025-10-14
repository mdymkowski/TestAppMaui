using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Maui.ApplicationModel;
using TestAppMaui.Application.Common.Models;
using TestAppMaui.Application.Tasks.Commands.CreateTask;
using TestAppMaui.Application.Tasks.Queries.GetTasks;

namespace TestAppMaui.MauiClient.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IMediator _mediator;

    public ObservableCollection<TaskDto> Tasks { get; } = new();

    [ObservableProperty]
    private string _newTaskTitle = string.Empty;

    [ObservableProperty]
    private string? _newTaskDescription;

    [ObservableProperty]
    private DateTime? _newTaskDueDate = DateTime.Today;

    public IAsyncRelayCommand CreateTaskCommand { get; }

    public MainViewModel(IMediator mediator)
    {
        _mediator = mediator;
        CreateTaskCommand = new AsyncRelayCommand(CreateTaskAsync);
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var tasks = await _mediator.Send(new GetTasksQuery());
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        });
    }

    private async Task CreateTaskAsync()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
        {
            return;
        }

        var command = new CreateTaskCommand(NewTaskTitle, NewTaskDescription, NewTaskDueDate);
        var createdTask = await _mediator.Send(command);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            Tasks.Add(createdTask);
            NewTaskTitle = string.Empty;
            NewTaskDescription = null;
            NewTaskDueDate = DateTime.Today;
        });
    }
}
