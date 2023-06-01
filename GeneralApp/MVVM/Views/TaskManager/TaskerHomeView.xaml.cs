using GeneralApp.MVVM.ViewModels;

namespace GeneralApp.MVVM.Views.TaskManager;

public partial class TaskerHomeView : ContentPage
{
    public TaskerHomeView(TaskerHomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}