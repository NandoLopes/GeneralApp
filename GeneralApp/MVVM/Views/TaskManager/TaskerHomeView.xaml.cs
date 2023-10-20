using GeneralApp.Abstractions.Interfaces;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.Views.TaskManager;

public partial class TaskerHomeView : ContentPage
{
    private readonly TaskerHomeViewModel _taskerHomeViewModel;
    private readonly INavigationService _navigationService;

    public TaskerHomeView(
        TaskerHomeViewModel taskerHomeViewModel,
        INavigationService navigationService
        )
    {
        InitializeComponent();
        _taskerHomeViewModel = taskerHomeViewModel;
        _navigationService = navigationService;
        BindingContext = _taskerHomeViewModel;
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await _navigationService.NavigateToPage<NewTaskView>(
                new Tuple<ObservableCollection<MyTask>, ObservableCollection<TaskCategory>>(_taskerHomeViewModel.Tasks, _taskerHomeViewModel.Categories)
            );
    }
}