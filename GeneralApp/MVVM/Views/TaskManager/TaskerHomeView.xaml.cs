using GeneralApp.MVVM.ViewModels;
using GeneralApp.Services;
using System.Text;

namespace GeneralApp.MVVM.Views.TaskManager;

public partial class TaskerHomeView : ContentPage
{
    private readonly TaskerHomeViewModel _taskerHomeViewModel;
    private readonly DialogService _dialogService;

    public TaskerHomeView(
        TaskerHomeViewModel taskerHomeViewModel,
        DialogService dialogService
        )
    {
        InitializeComponent();
        _taskerHomeViewModel = taskerHomeViewModel;
        _dialogService = dialogService;
        BindingContext = _taskerHomeViewModel;
    }

    private async void DeleteTasks(object sender, EventArgs e)
    {
        var tasksToDeleteCount = _taskerHomeViewModel.SelectedTasks.Count;
        bool confirmDelete = await DisplayAlert("CONFIRM", $"Delete {tasksToDeleteCount} tasks?", "Yes", "No");

        if (!confirmDelete) return;

        var errors = await _taskerHomeViewModel.DeleteTasks();

        if (errors.Any())
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append("Error trying to delete task(s) with Id(s): ");
            foreach ( var error in errors )
            {
                stringBuilder.Append($"{error.Item1.Id}, ");
            }
            stringBuilder.Append("sorry :(");

            await _dialogService.SnackbarErrorAsync(stringBuilder.ToString());
        } else
        {
            await _dialogService.SnackbarSuccessAsync("Task(s) deleted!");
        }

        _taskerHomeViewModel.SelectedCategories.Clear();
        _taskerHomeViewModel.RefreshTasks();
    }
}