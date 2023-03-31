using GeneralApp.MVVM.Home;
using GeneralApp.MVVM.Views.TaskManager;

namespace GeneralApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new NavigationPage(new Home());  //<--- Official
		//MainPage = new NavigationPage(new NewTaskView());
	}
}
