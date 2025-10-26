using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using MediatR;
using Microsoft.Maui.ApplicationModel;
using TestAppMaui.MauiClient.Application.Tasks;
using TestAppMaui.MauiClient.Application.Tasks.Commands.CreateTask;
using TestAppMaui.MauiClient.Application.Tasks.Queries;

namespace TestAppMaui.MauiClient.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IMediator _mediator;

    public ObservableCollection<TaskDto> Tasks { get; } = new();

    [ObservableProperty]
    private string _newTaskName = string.Empty;

    [ObservableProperty]
    private string? _newTaskDescription = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    public IAsyncRelayCommand SendCommand { get; }

    public MainViewModel(IMediator mediator)
    {
        _mediator = mediator;
        SendCommand = new AsyncRelayCommand(SendAsync);
        _ = InitializeAsync();
    }

    partial void OnNewTaskNameChanged(string value)
    {
        NewTaskDescription = $"Domyœlny opis dla: {value}";
    }

    private async Task InitializeAsync()
    {
        try
        {
            IsBusy = true;
            var tasks = await _mediator.Send(new GetTasksQuery()).ConfigureAwait(false);
            UpdateTasks(tasks);
            ErrorMessage = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task SendAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            var createdTask = await _mediator.Send(new CreateTaskCommand(NewTaskName, NewTaskDescription)).ConfigureAwait(false);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var insertIndex = Tasks.TakeWhile(task => string.Compare(task.Name, createdTask.Name, StringComparison.CurrentCultureIgnoreCase) <= 0).Count();
                Tasks.Insert(insertIndex, createdTask);
                NewTaskName = string.Empty;
                NewTaskDescription = string.Empty;
            });

            ErrorMessage = null;
        }
        catch (ValidationException validationException)
        {
            ErrorMessage = validationException.Errors.FirstOrDefault()?.ErrorMessage ?? validationException.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateTasks(IEnumerable<TaskDto> tasks)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Tasks.Clear();
            foreach (var task in tasks.OrderBy(task => task.Name, StringComparer.CurrentCultureIgnoreCase))
            {
                Tasks.Add(task);
            }
        });
    }
}