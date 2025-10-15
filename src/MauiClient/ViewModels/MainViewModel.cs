using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using TestAppMaui.MauiClient.Application.Abstractions;
using TestAppMaui.MauiClient.Domain;
using TestAppMaui.SharedDDD.Contracts;

namespace TestAppMaui.MauiClient.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IGatewayApiClient _gatewayApiClient;
    private readonly ILocalTaskStore _localTaskStore;

    public ObservableCollection<TaskDto> Tasks { get; } = new();

    [ObservableProperty]
    private string _newTaskTitle = string.Empty;

    [ObservableProperty]
    private string? _newTaskDescription;

    [ObservableProperty]
    private DateTime? _newTaskDueDate = DateTime.Today;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _errorMessage;

    public IAsyncRelayCommand CreateTaskCommand { get; }

    public IAsyncRelayCommand RefreshCommand { get; }

    public MainViewModel(IGatewayApiClient gatewayApiClient, ILocalTaskStore localTaskStore)
    {
        _gatewayApiClient = gatewayApiClient;
        _localTaskStore = localTaskStore;

        CreateTaskCommand = new AsyncRelayCommand(CreateTaskAsync);
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await _localTaskStore.InitializeAsync().ConfigureAwait(false);
            var localTasks = await _localTaskStore.GetTasksAsync().ConfigureAwait(false);
            UpdateTasks(localTasks);

            await RefreshAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private async Task CreateTaskAsync()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(NewTaskTitle))
        {
            return;
        }

        try
        {
            IsBusy = true;

            var request = new CreateTaskRequest(NewTaskTitle, NewTaskDescription, NewTaskDueDate);
            var createdTask = await _gatewayApiClient.CreateTaskAsync(request).ConfigureAwait(false);
            await _localTaskStore.AddOrUpdateTaskAsync(createdTask).ConfigureAwait(false);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Tasks.Add(createdTask);
                NewTaskTitle = string.Empty;
                NewTaskDescription = null;
                NewTaskDueDate = DateTime.Today;
            });

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

    private async Task RefreshAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;

            var remoteTasks = await _gatewayApiClient.GetTasksAsync().ConfigureAwait(false);
            await _localTaskStore.ReplaceTasksAsync(remoteTasks).ConfigureAwait(false);
            UpdateTasks(remoteTasks);

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

    private void UpdateTasks(IEnumerable<TaskDto> tasks)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Tasks.Clear();
            foreach (var task in tasks.OrderBy(t => t.DueDate ?? DateTime.MaxValue))
            {
                Tasks.Add(task);
            }
        });
    }
}
