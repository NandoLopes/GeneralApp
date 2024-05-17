using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels.TaskManager;
using GeneralApp.Services;

namespace GeneralApp.MVVM.Views.TaskManager;

public partial class NewTaskView : ContentPage
{
    private readonly NewTaskViewModel _viewModel;
    private readonly DialogService _dialogService;

    public NewTaskView(NewTaskViewModel viewModel,
					   DialogService dialogService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _dialogService = dialogService;
        BindingContext = _viewModel;
    }

    private async void SaveItemClicked(object sender, EventArgs e)
    {
        _viewModel.NewTask.Category = _viewModel.SelectedCategory;

        var taskSaveResult = await _viewModel.SaveButton();

        if (taskSaveResult.HasError)
        {
            await DisplayAlert("Error", taskSaveResult.StatusMessage, "Ok");
            return;
        }

        var categoryUpdateResult = await _viewModel.UpdateCategoryInfo(taskSaveResult.Result.CategoryId);

        if (categoryUpdateResult.HasError)
        {
            await DisplayAlert("Error", categoryUpdateResult.StatusMessage, "Ok");
            return;
        }

        await _dialogService.SnackbarSuccessAsync("Task saved!");
        await Navigation.PopAsync();
    }

    private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;

        if (picker.SelectedItem == null)
        {
            _viewModel.Validate();
            return;
        }
        else if (picker.SelectedIndex > 0)
        {
            _viewModel.NewTask.Category = _viewModel.SelectedCategory;
            _viewModel.Validate();
            return;
        }

        string category =
            await DisplayPromptAsync("New Category",
            "Write the new category name",
            maxLength: 15,
            keyboard: Keyboard.Text);

        if (category == null)
        {
            _viewModel.Validate();
            return;
        }
        else if (string.IsNullOrEmpty(category.Trim()))
        {
            await DisplayAlert("Error", "The new Category needs a name!", "Ok");
            picker.SelectedItem = null;
            _viewModel.Validate();
            return;
        }

        //TODO - Chose color.
        var color = System.Drawing.Color.FromArgb(category.GetHashCode());

        var result = await _viewModel.AddCategory(new TaskCategory
        {
            Name = category.Trim(),
            Color = Color.FromRgb(color.R, color.G, color.B).ToHex()
        });

        if (result.HasError)
        {
            await DisplayAlert("Error", result.StatusMessage, "Ok");
            _viewModel.Validate();
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Category created!");

            _viewModel.NewTask.Category = _viewModel.Categories.FirstOrDefault(x => x.Name == category);
            _viewModel.SelectedCategory = _viewModel.NewTask.Category;
            _viewModel.Validate();
        }
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.Validate();
    }

    private void DateCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkBox = sender as CheckBox;

        if (!checkBox.IsChecked)
        {
            _viewModel.NewTask.DueDate = null;
        }
        else if (checkBox.IsChecked)
        {
            _viewModel.NewTask.DueDate = DateTime.Now.AddDays(7);
        }
    }
}