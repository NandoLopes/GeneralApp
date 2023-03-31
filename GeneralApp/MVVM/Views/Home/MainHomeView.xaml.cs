using GeneralApp.MVVM.ViewModels;
using GeneralApp.MVVM.Views.TaskManager;

namespace GeneralApp.MVVM.Home;

public partial class Home : ContentPage
{
	public Home()
	{
		InitializeComponent();

		BindingContext = new HomeViewModel();
	}

	private void ButtonTaskPage(object sender, EventArgs e)
	{
		var button = (Button)sender;

		var newView =  button.Text switch
		{
            "Task Manager" => new TaskerHomeView(),
            "Calculator" => null,
            "Not Implemented" => null,
            "To be done" => null,
			_ => null
        };

		if(newView != null)
		{
			Navigation.PushAsync(newView);
		} else
		{
			DisplayAlert("Sorry", "The page you're looking for was not implemented yet.", "Ok");
        }
	}
}