using GeneralApp.MVVM.ViewModels;
using GeneralApp.MVVM.ViewModels.TaskManager;

namespace GeneralApp.MVVM.Views.TaskManager;

public partial class TaskerHomeView : ContentPage
{
    private TaskerHomeViewModel taskerHomeViewModel = new TaskerHomeViewModel();

    public TaskerHomeView()
    {
        InitializeComponent();
        BindingContext = taskerHomeViewModel;
    }

    private void checkBox_CheckedChanged(object sender, EventArgs e)
    {
        taskerHomeViewModel.UpdateData();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var taskView = new NewTaskView
        {
            BindingContext = new NewTaskViewModel
            {
                Tasks = taskerHomeViewModel.Tasks,
                Categories = taskerHomeViewModel.Categories
            }
        };

        Navigation.PushAsync(taskView);
    }
}