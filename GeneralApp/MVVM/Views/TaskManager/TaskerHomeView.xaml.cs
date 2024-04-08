using GeneralApp.MVVM.Models;
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
            foreach (var error in errors)
            {
                stringBuilder.Append($"{error.Item1.Id}, ");
            }
            stringBuilder.Append("sorry :(");

            await _dialogService.SnackbarErrorAsync(stringBuilder.ToString());
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Task(s) deleted!");
        }

        _taskerHomeViewModel.SelectedCategories.Clear();
        _taskerHomeViewModel.RefreshTasks();
    }

    private async void DeleteCategories(object sender, EventArgs e)
    {
        var categoriesToDeleteCount = _taskerHomeViewModel.SelectedCategories.Count;
        bool confirmDelete = await DisplayAlert("CONFIRM", $"Delete {categoriesToDeleteCount} categories? This will also delete their tasks.", "Yes", "No");

        if (!confirmDelete) return;

        var errors = _taskerHomeViewModel.DeleteCategories();

        if (errors.Any())
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append("Error trying to delete categories with Ids: ");
            foreach (var error in errors)
            {
                stringBuilder.Append($"{error.Item1.Id}, ");
            }
            stringBuilder.Append("sorry :(");

            await _dialogService.SnackbarErrorAsync(stringBuilder.ToString());
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Categories deleted!");
        }

        _taskerHomeViewModel.SelectedCategories.Clear();
        _taskerHomeViewModel.RefreshCategories();
    }

    private async void AddCategoryClicked(object sender, EventArgs e)
    {
        if (_taskerHomeViewModel.SelectedCategories.Count > 1) return;

        var oldCategory = _taskerHomeViewModel.SelectedCategories.FirstOrDefault() as TaskCategory;

        string category =
            await DisplayPromptAsync("Edit Category",
            "Write the category name",
            maxLength: 15,
            keyboard: Keyboard.Text,
            initialValue: oldCategory.Name);

        if (category == null || category?.Trim() == oldCategory.Name)
        {
            return;
        }
        else if (string.IsNullOrEmpty(category.Trim()))
        {
            await DisplayAlert("Error", "The new Category needs a name!", "Ok");
            return;
        }

        oldCategory.Name = category;

        var result = await _taskerHomeViewModel.AddCategory(oldCategory);

        if (result.HasError)
        {
            await DisplayAlert("Error", result.StatusMessage, "Ok");
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Category created!");
        }
    }
}