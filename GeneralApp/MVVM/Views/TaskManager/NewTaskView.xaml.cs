using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels.TaskManager;
using GeneralApp.Services;

namespace GeneralApp.MVVM.Views.TaskManager;

public partial class NewTaskView : ContentPage
{
    private readonly NewTaskViewModel _newTaskViewModel;
    private readonly DialogService _dialogService;

    public NewTaskView(NewTaskViewModel newTaskViewModel,
					   DialogService dialogService)
	{
		InitializeComponent();
        _newTaskViewModel = newTaskViewModel;
        _dialogService = dialogService;
        BindingContext = _newTaskViewModel;
    }

    private async void AddTaskClicked(object sender, EventArgs e)
    {
		//Updated old category if edit

		var selectedCategory =
			_newTaskViewModel.SelectedCategory;

        if (string.IsNullOrEmpty(_newTaskViewModel.NewTask.Name.Trim()))
        {
            await DisplayAlert("Empty Field", "You must fill the task field", "Ok");
			return;
        }

        if (selectedCategory == null)
        {
            await DisplayAlert("Invalid Selection", "You must select a category", "Ok");
        }
		else if (selectedCategory != null && !string.IsNullOrEmpty(_newTaskViewModel.NewTask.Name.Trim()))
		{
            _newTaskViewModel.NewTask.Name = _newTaskViewModel.NewTask.Name.Trim();

			var result = await _newTaskViewModel.AddTask();

            if (result.HasError)
            {
                await DisplayAlert("Error", result.StatusMessage, "Ok");
                return;
            }

            await _dialogService.SnackbarSuccessAsync("Item saved!");
            await Navigation.PopAsync();
		}
		else
		{
            await DisplayAlert("Error", "Something went wrong :(", "Ok");
        }
    }

    private async void AddCategoryClicked(object sender, EventArgs e)
    {
		string category =
			await DisplayPromptAsync("New Category",
			"Write the new category name",
			maxLength: 15,
			keyboard: Keyboard.Text);

		if (category == null)
		{
			return;
		}
		else if (string.IsNullOrEmpty(category.Trim()))
		{
            await DisplayAlert("Error", "The new Category needs a name!", "Ok");
			return;
		}

		//TODO - Chose color.
		var r = new Random();

		var result = await _newTaskViewModel.AddCategory(new TaskCategory
		{
			Name = category.Trim(),
			Color = Color.FromRgb(
				r.Next(0, 255),
				r.Next(0, 255),
				r.Next(0, 255)).ToHex()
		});

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