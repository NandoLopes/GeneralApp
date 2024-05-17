using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels;
using GeneralApp.Services;
using System.Text;

namespace GeneralApp.MVVM.Views.TaskManager;

public partial class TaskerHomeView : ContentPage
{
    private readonly TaskerHomeViewModel _viewModel;
    private readonly DialogService _dialogService;

    public TaskerHomeView(
        TaskerHomeViewModel taskerHomeViewModel,
        DialogService dialogService
        )
    {
        InitializeComponent();
        _viewModel = taskerHomeViewModel;
        _dialogService = dialogService;
        BindingContext = _viewModel;
        _viewModel.ScrollRequested += ScrollRequested;
    }

    ~TaskerHomeView() 
    {
        _viewModel.ScrollRequested -= ScrollRequested;
    }

    private void ScrollRequested(object sender, int index)
    {
        taskCollectionView.ScrollTo(index);
    }

    private async void AddCategoryClicked(object sender, EventArgs e)
    {
        var category = _viewModel.SelectedCategories.Count == 1 ? _viewModel.SelectedCategories[0] as TaskCategory : new();
        string categoryName =
            (await DisplayPromptAsync("New Category",
            "Write the new category name",
            maxLength: 15,
            keyboard: Keyboard.Text,
            initialValue: category.Name))?.Trim();

        if (categoryName == null)
        {
            return;
        }
        else if (string.IsNullOrEmpty(categoryName))
        {
            await DisplayAlert("Error", "The Category needs a name!", "Ok");
            return;
        }

        category.Name = categoryName;

        //TODO - Chose color.
        if (category.Id == 0)
        {
            var color = System.Drawing.Color.FromArgb(categoryName.GetHashCode());
            category.Color = Color.FromRgb(color.R, color.G, color.B).ToHex();
        }

        var result = await _viewModel.AddCategory(category);

        if (result.HasError)
        {
            await DisplayAlert("Error", result.StatusMessage, "Ok");
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Category created!");
        }
    }

    private async void DeleteTasks(object sender, EventArgs e)
    {
        var tasksToDeleteCount = _viewModel.SelectedTasks.Count;
        bool confirmDelete = await DisplayAlert("CONFIRM", $"Delete {tasksToDeleteCount} tasks?", "Yes", "No");

        if (!confirmDelete) return;

        var errors = await _viewModel.DeleteTasks();

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

        _viewModel.SelectedCategories.Clear();
        _viewModel.SelectedTasks.Clear();
        _viewModel.RefreshCategories();
        _viewModel.RefreshTasks();
    }

    private async void DeleteCategories(object sender, EventArgs e)
    {
        var categoriesToDeleteCount = _viewModel.SelectedCategories.Count;
        bool confirmDelete = await DisplayAlert("CONFIRM", $"Delete {categoriesToDeleteCount} categories? This will also delete their tasks.", "Yes", "No");

        if (!confirmDelete) return;

        var errors = _viewModel.DeleteCategories();

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

        _viewModel.SelectedCategories.Clear();
        _viewModel.RefreshCategories();
    }
}