using CommunityToolkit.Mvvm.Input;
using GeneralApp.Interfaces;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Views.ColorPicker;
using GeneralApp.MVVM.Views.TaskManager;
using GeneralApp.Services;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels
{
    public partial class HomeViewModel : ViewModelBase
    {
        public ObservableCollection<AppFunction> FunctionsList { get; set; }
        private readonly INavigationService _navigationService;
        private readonly DialogService _dialogService;

        public HomeViewModel(INavigationService navigationService, DialogService dialogService)
        {
            FillData();
            _navigationService = navigationService;
            _dialogService = dialogService;
        }

        [RelayCommand]
        private async Task NavigateButton(AppFunction item)
        {
            if (item.Name == "Task Manager")
                await _navigationService.NavigateToPage<TaskerHomeView>();
            else if (item.Name == "Color Picker")
                await _navigationService.NavigateToPage<ColorPickerView>();
            else
                await _dialogService.SnackbarWarningAsync("Sorry, the page you're looking for was not implemented yet.");
        }

        private void FillData()
        {
            FunctionsList = new ObservableCollection<AppFunction>
            {
                new AppFunction{ Id = 1, Name = "Task Manager" },
                new AppFunction { Id = 2, Name = "Calculator" },
                new AppFunction { Id = 4, Name = "Color Picker" },
                new AppFunction { Id = 3, Name = "Not Implemented" }
            };
        }
    }
}
