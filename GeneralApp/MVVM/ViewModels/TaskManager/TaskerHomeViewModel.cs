using CommunityToolkit.Mvvm.Input;
using GeneralApp.Abstractions.Interfaces;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Views.Stock;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public partial class TaskerHomeViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ObservableCollection<TaskCategory> Categories { get; set; }
        public ObservableCollection<MyTask> Tasks { get; set; }

        public IAsyncRelayCommand PullToRefreshCommand { get; set; }

        public TaskerHomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            Categories = new();
            Tasks = new();

            PullToRefreshCommand = new AsyncRelayCommand(ExecPullToRefreshCommand, CanExecPullToRefreshCommand);
        }

        private bool _canExecPullToRefreshCommand = true;

        private bool CanExecPullToRefreshCommand() => _canExecPullToRefreshCommand;
        private async Task ExecPullToRefreshCommand()
        {
            if (_canExecPullToRefreshCommand)
            {
                _canExecPullToRefreshCommand = false;

                await FillData();
                //Delay to check functionality
                await Task.Delay(1000);
                IsRefreshing = false;
                _canExecPullToRefreshCommand = true;
            }
        }

        [RelayCommand]
        private async Task Appearing()
        {
            await FillData();
        }

        [RelayCommand]
        private async Task CheckedChanged(MyTask task)
        {
            if (task == null) return;
                
            var categoryIndex = Categories.IndexOf(Categories.Where(x => x.Id == task.CategoryId).FirstOrDefault());

            await App.TaskRepo.SaveItem(task);

            var category = App.TaskCategoryRepo.GetItemWithChildren(task.CategoryId);
            if (category.HasError) return;

            int completedTasks = category.Result.MyTasks.Where(x => x.Completed).Count();
            int pendingTasks = category.Result.MyTasks.Where(x => x.Completed == false).Count();

            category.Result.PendingTasks = pendingTasks;
            category.Result.Percentage = (float)completedTasks / (float)category.Result.MyTasks.Count;

            await App.TaskCategoryRepo.SaveItem(category.Result);

            Categories[categoryIndex] = category.Result;
        }

        private Task FillData()
        {
            RefreshCategories();
            RefreshTasks();

            return Task.CompletedTask;
        }

        private void RefreshCategories()
        {
            var categories = App.TaskCategoryRepo.GetItemsWithChildren();

            if (categories.HasError || categories.Result.Count == 0)
            {
                Categories = new();
                return;
            }

            Categories.Clear();
            foreach (var category in categories.Result)
            {
                Categories.Add(category);
            }
        }

        private void RefreshTasks()
        {
            var tasks = App.TaskRepo.GetItemsWithChildren();

            if (tasks.HasError || tasks.Result.Count == 0)
            {
                Tasks = new();
                return;
            }

            Tasks.Clear();
            foreach (var task in tasks.Result)
            {
                Tasks.Add(task);
            }
        }

        [RelayCommand]
        private async void Stock()
        {
            await _navigationService.NavigateToPage<StockHomeView>();
        }
    }
}