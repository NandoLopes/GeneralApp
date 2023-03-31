using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels.TaskManager;

namespace GeneralApp.MVVM.Views.TaskManager;

public partial class NewTaskView : ContentPage
{
	public NewTaskView()
	{
		InitializeComponent();
	}

    private async void AddTaskClicked(object sender, EventArgs e)
    {
		var viewModel = BindingContext as NewTaskViewModel;


		//TODO - Remove "IsSelected" property.
		var selectedCategory =
			viewModel.Categories.Where(x => x.IsSelected == true).FirstOrDefault();

		if (string.IsNullOrEmpty(viewModel.Task))
		{
            await DisplayAlert("Empty Field", "You must fill the task field", "Ok");
        }
		else if (selectedCategory == null)

        {
			await DisplayAlert("Invalid Selection", "You must select a category", "Ok");
		}
		else if (selectedCategory != null && !string.IsNullOrEmpty(viewModel.Task))
		{
			var task = new MyTask
			{
				Id = viewModel.Tasks.Count + 1,
				Name = viewModel.Task,
				CategoryId = selectedCategory.Id
			};

			viewModel.Tasks.Add(task);
			await Navigation.PopAsync();
		}
		else
		{
            await DisplayAlert("Error", "Something went wrong :(", "Ok");
        }
    }

    private async void AddCategoryClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as NewTaskViewModel;

		string category =
			await DisplayPromptAsync("New Category",
			"Write the new category name",
			maxLength: 15,
			keyboard: Keyboard.Text);

		//TODO - Chose color.
		var r = new Random();

		if(!string.IsNullOrEmpty(category))
		{
			viewModel.Categories.Add(new Category
			{
				Id = viewModel.Categories.Count + 1,
				Name = category,
				Color = Color.FromRgb(
					r.Next(0, 255),
					r.Next(0, 255),
					r.Next(0, 255)).ToHex()
			});
		}
    }
}