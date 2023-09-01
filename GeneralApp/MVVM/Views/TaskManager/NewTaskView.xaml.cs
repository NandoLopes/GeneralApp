using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels.TaskManager;

namespace GeneralApp.MVVM.Views.TaskManager;

public partial class NewTaskView : ContentPage
{
    private readonly NewTaskViewModel _newTaskViewModel;

    public NewTaskView(NewTaskViewModel newTaskViewModel)
	{
		InitializeComponent();
        _newTaskViewModel = newTaskViewModel;
		BindingContext = _newTaskViewModel;
    }

    private async void AddTaskClicked(object sender, EventArgs e)
    {
		//TODO - Remove "IsSelected" property.
		var selectedCategory =
            _newTaskViewModel.Categories.Where(x => x.IsSelected == true).FirstOrDefault();

		if (string.IsNullOrEmpty(_newTaskViewModel.NewTask))
		{
            await DisplayAlert("Empty Field", "You must fill the task field", "Ok");
        }
		else if (selectedCategory == null)

        {
			await DisplayAlert("Invalid Selection", "You must select a category", "Ok");
		}
		else if (selectedCategory != null && !string.IsNullOrEmpty(_newTaskViewModel.NewTask))
		{
			var task = new MyTask
			{
				Id = _newTaskViewModel.Tasks.Count + 1,
				Name = _newTaskViewModel.NewTask,
				CategoryId = selectedCategory.Id
			};

			_newTaskViewModel.Tasks.Add(task);
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

		//TODO - Chose color.
		var r = new Random();

		if(!string.IsNullOrEmpty(category))
		{
			_newTaskViewModel.Categories.Add(new Category
			{
				Id = _newTaskViewModel.Categories.Count + 1,
				Name = category,
				Color = Color.FromRgb(
					r.Next(0, 255),
					r.Next(0, 255),
					r.Next(0, 255)).ToHex()
			});
		}
    }
}