using CommunityToolkit.Mvvm.Input;
using GeneralApp.Abstractions.Interfaces;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Models.Helpers;
using GeneralApp.MVVM.Views.Stock;
using GeneralApp.MVVM.Views.TaskManager;
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
        public ObservableCollection<object> SelectedCategories { get; set; } //.NET MAUI bug won't update SelectedItems if not in an "object" type list.
        public ObservableCollection<object> SelectedTasks { get; set; } //.NET MAUI bug won't update SelectedItems if not in an "object" type list.

        public bool VisibleClearCategory { get; set; }
        public bool VisibleClearTask { get; set; }
        public bool VisibleEditCategory { get; set; }
        public bool VisibleEditTask { get; set; }


        public TaskerHomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            Categories = new();
            SelectedCategories = new();
            SelectedTasks = new();
            Tasks = new();
        }

        private bool _canExecPullToRefreshCommand = true;

        private bool CanExecPullToRefreshCommand() => _canExecPullToRefreshCommand;
        [RelayCommand(CanExecute = nameof(CanExecPullToRefreshCommand))]
        private async Task PullToRefresh()
            {
                _canExecPullToRefreshCommand = false;

                await FillData();
                IsRefreshing = false;
                _canExecPullToRefreshCommand = true;
            }

        [RelayCommand]
        private async Task Appearing()
        {
            _canExecuteCommands = false;

            SelectedCategories = new();
            SelectedTasks = new();
            VisibleClearCategory = false;
            VisibleClearTask = false;
            VisibleEditCategory = false;
            VisibleEditTask = false;
            await FillData();

            _canExecuteCommands = true;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
        private void CategorySelectionChanged()
        {
            try
            {
                _canExecuteCommands = false;

                ClearTaskSelection();
                UpdateCategorySelection();
                RefreshTasks();
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
        private void TaskSelectionChanged()
        {
            try
            {
                _canExecuteCommands = false;
                UpdateTaskSelection();
            }
            finally
            {
                _canExecuteCommands = true;
            }
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

                /*Next two lines added because MAUI will trigger the command with a category existing in the list,
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
                await FillData();
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        [RelayCommand]
        private void ClearTaskSelection()
        {
            try
            {
                _canExecuteCommands = false;

                SelectedTasks.Clear();
                UpdateTaskSelection();
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
            SelectedTasks.Clear();
            RefreshTasks();

            return Task.CompletedTask;
        }

        public void RefreshCategories()
        {
            try
            {
                _canExecuteCommands = false;

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
            finally
            {
                _canExecuteCommands = true;
            }
        }

        public void RefreshTasks()
        {
            try
            {
                _canExecuteCommands = false;

                var selectedCategories = SelectedCategories.Cast<TaskCategory>().ToList();

                var it = App.TaskRepo.GetItemsWithChildrenPredicate(x => true);

                var tasks = App.TaskRepo.GetItemsWithChildrenPredicate(x => selectedCategories.Any(y => y.Id == x.CategoryId) || selectedCategories.Count == 0);

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
            finally
            {
                UpdateTaskSelection();
                _canExecuteCommands = true;
            }
        }

        private void UpdateCategorySelection()
        {
            try
            {
                _canExecuteCommands = false;

                VisibleClearCategory = SelectedCategories.Any();
                VisibleEditCategory = SelectedCategories.Count == 1;

                foreach (var category in Categories)
                {
                    category.IsSelected = SelectedCategories.Cast<TaskCategory>().Any(y => y.Id == category.Id);
                }
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        private void UpdateTaskSelection()
        {
            try
            {
                _canExecuteCommands = false;

                VisibleClearTask = SelectedTasks.Any();
                VisibleEditTask = SelectedTasks.Count == 1;

                foreach (var task in Tasks)
                {
                    task.IsSelected = SelectedTasks.Cast<MyTask>().Any(y => y.Id == task.Id);
                }
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        [RelayCommand]
        private async void TaskEdit(string isEdit)
        {
            await _navigationService.NavigateToPage<NewTaskView>(
                (isEdit == "edit" && SelectedTasks?.Count == 1) ? SelectedTasks.FirstOrDefault() : null
            );
        }

        public async Task<GenericResponse<TaskCategory>> AddCategory(TaskCategory newCategory)
        {
            var verify = App.TaskCategoryRepo.GetItem(x => x.Name.ToLower() == newCategory.Name.ToLower());
            if (verify.Result != null) return new GenericResponse<TaskCategory> { HasError = true, StatusMessage = $"Category {newCategory.Name} already exists." };

            await App.TaskCategoryRepo.SaveItem(newCategory);

            SelectedCategories.Clear();
            RefreshCategories();
            return new GenericResponse<TaskCategory>();
        }

        public async Task<List<(MyTask, GenericResponse<MyTask>)>> DeleteTasks()
        {
            List<(MyTask, GenericResponse<MyTask>)> errorTask = new();
            List<int> differentCategoryIds = new();

            foreach (MyTask task in SelectedTasks.Cast<MyTask>())
            {
                var result = App.TaskRepo.DeleteItem(task, true);

                if (result.HasError) errorTask.Add((task, result));
                else if (!differentCategoryIds.Any(x => x == task.CategoryId))
                {
                    differentCategoryIds.Add(task.CategoryId);
                }
            }

            foreach (int categoryId in differentCategoryIds)
            {
                await UpdateCategoryInfo(categoryId);
            }

            return errorTask;
        }

        public List<(TaskCategory, GenericResponse<TaskCategory>)> DeleteCategories()
        {
            List<(TaskCategory, GenericResponse<TaskCategory>)> errorCategory = new();

            foreach (TaskCategory category in SelectedCategories.Cast<TaskCategory>())
            {
                var result = App.TaskCategoryRepo.DeleteItem(category, true);

                if (result.HasError) errorCategory.Add((category, result));
            }

            return errorCategory;
        }

        private static async Task<GenericResponse<TaskCategory>> UpdateCategoryInfo(int categoryId)
        {
            var category = App.TaskCategoryRepo.GetItemWithChildren(categoryId);

            if (category.HasError || category.Result == null) return category;

            if (category.Result.MyTasks.Count == 0)
            {
                category.Result.Percentage = 1;
                category.Result.PendingTasks = 0;
                return await App.TaskCategoryRepo.SaveItem(category.Result);
            }

            var categoryTasks = category.Result.MyTasks ??= new();
            var completedTasks = category.Result.MyTasks.Where(x => x.Completed).Count();

            category.Result.PendingTasks = category.Result.MyTasks.Count - completedTasks;
            category.Result.Percentage = (float)completedTasks / (float)categoryTasks.Count;

            var result = await App.TaskCategoryRepo.SaveItem(category.Result);

            return result;
        }

        [RelayCommand]
        private async void Stock()
        {
            await _navigationService.NavigateToPage<StockHomeView>();
        }
    }
}