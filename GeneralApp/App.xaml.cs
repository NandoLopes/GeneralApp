using GeneralApp.MVVM.Home;
using GeneralApp.MVVM.Views.TaskManager;

namespace GeneralApp;

public partial class App : Application
{
	public App(Home home)
	{
		InitializeComponent();

		MainPage = new NavigationPage(home);  //<--- Official
		//MainPage = new NavigationPage(new NewTaskView());
	}
}
