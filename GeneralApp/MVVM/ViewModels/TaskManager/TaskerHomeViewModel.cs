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
        public ObservableCollection<object> SelectedCategories { get; set; } //.NET MAUI bug won't update SelectedItems if not in an "object" type list.
        public ObservableCollection<MyTask> Tasks { get; set; }

        public bool VisibleClearCategory { get; set; }

        public IAsyncRelayCommand PullToRefreshCommand { get; set; }

        public TaskerHomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            Categories = new();
            SelectedCategories = new();
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
            _canExecuteCommands = false;

            SelectedCategories = new();
            VisibleClearCategory = false;
            await FillData();

            _canExecuteCommands = true;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
        private void CategorySelectionChanged()
        {
            _canExecuteCommands = false;

            VisibleClearCategory = SelectedCategories.Any();
            UpdateCategorySelection();
            RefreshTasks();

            _canExecuteCommands = true;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
        private async Task CheckedChanged(MyTask task)
        {
            try
            {
                _canExecuteCommands = false;

                if (task == null)
                {
                    _canExecuteCommands = true;
                    return;
                }

              /*Next two lines added because MAUI will trigger the command with a task existing in the list,
                but with the Completed flag inverted. CanExecute on the command didn't stopped this from triggering. */
                var existingTask = Tasks.FirstOrDefault(x => x.Id == task.Id);
                if (existingTask != null && task.Completed != existingTask.Completed)
                {
                    _canExecuteCommands = true;
                    return;
                }

                var categoryIndex = Categories.IndexOf(Categories.Where(x => x.Id == task.CategoryId).FirstOrDefault());
                await App.TaskRepo.SaveItem(task);

                var category = App.TaskCategoryRepo.GetItemWithChildren(task.CategoryId);
                if (category.HasError)
                {
                    _canExecuteCommands = true;
                    return;
                }

                int completedTasks = category.Result.MyTasks.Where(x => x.Completed).Count();
                int pendingTasks = category.Result.MyTasks.Where(x => x.Completed == false).Count();

                category.Result.PendingTasks = pendingTasks;
                category.Result.Percentage = (float)completedTasks / (float)category.Result.MyTasks.Count;

                await App.TaskCategoryRepo.SaveItem(category.Result);

                if (SelectedCategories.Cast<TaskCategory>().ToList().FindIndex(x => x.Id == category.Result.Id) is int index && index != -1)
                {
                    SelectedCategories[index] = category.Result;
                    category.Result.IsSelected = true;
                }

                Categories[categoryIndex] = category.Result;
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        [RelayCommand]
        private async void ClearCategorySelection()
        {
            try
            {
                _canExecuteCommands = false;

                SelectedCategories.Clear();
                VisibleClearCategory = false;
                await FillData();
            } 
            finally
            {
                _canExecuteCommands = true;
            }
        }

        private Task FillData()
        {
            RefreshCategories();
            SelectedCategories.Clear();
            VisibleClearCategory = false;
            UpdateCategorySelection();
            RefreshTasks();

            return Task.CompletedTask;
        }

        private void RefreshCategories()
        {
            _canExecuteCommands = false;

            var categories = App.TaskCategoryRepo.GetItemsWithChildren();

            if (categories.HasError || categories.Result.Count == 0)
            {
                Categories = new();
            }

            Categories.Clear();
            foreach (var category in categories.Result)
            {
                Categories.Add(category);
            }

            _canExecuteCommands = true;
        }

        private void RefreshTasks()
        {
            _canExecuteCommands = false;

            var selectedCategories = SelectedCategories.Cast<TaskCategory>().ToList();
            var tasks = App.TaskRepo.GetItemsWithChildrenPredicate(x => selectedCategories.Any(y => y.Id == x.Category.Id) || selectedCategories.Count == 0);

            if (tasks.HasError || tasks.Result.Count == 0)
            {
                Tasks = new();
            }

            Tasks.Clear();
            foreach (var task in tasks.Result)
            {
                Tasks.Add(task);
            }

            _canExecuteCommands = true;
        }

        private void UpdateCategorySelection()
        {
            _canExecuteCommands = false;

            foreach (var category in Categories)
            {
                category.IsSelected = SelectedCategories.Cast<TaskCategory>().Any(y => y.Id == category.Id);
            }

            _canExecuteCommands = true;
        }

        [RelayCommand]
        private async void Stock()
        {
            await _navigationService.NavigateToPage<StockHomeView>();
        }
    }
}