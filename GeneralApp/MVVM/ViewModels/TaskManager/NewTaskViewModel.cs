using CommunityToolkit.Mvvm.Input;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Models.Helpers;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels.TaskManager
{
    public partial class NewTaskViewModel : ViewModelBase
    {
        public ObservableCollection<TaskCategory> Categories { get; set; }
        public TaskCategory SelectedCategory { get; set; }
        public MyTask NewTask { get; set; }

        public override Task OnNavigatingTo(object? parameter)
        {
            Categories = new();
            NewTask = new();
            SelectedCategory = new();

            if (parameter == null)
            {
                Title = "Add Task";
                return Task.CompletedTask;
            } 
            else if (parameter is MyTask newparameter)
            {
                Title = "Edit Task";
                NewTask = newparameter;
                SelectedCategory = NewTask.Category ?? new();
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        private void Appearing()
        {
            RefreshCategories();
        }

        [RelayCommand]
        private void CategorySelectionChanged()
        {
            UpdateCategorySelection();
        }

        public async Task<GenericResponse<TaskCategory>> AddCategory(TaskCategory newCategory)
        {
            var verify = App.TaskCategoryRepo.GetItem(x => x.Name.ToLower() == newCategory.Name.ToLower());
            if (verify.Result != null) return new GenericResponse<TaskCategory> { HasError = true, StatusMessage = $"Category {newCategory.Name} already exists." };

            await App.TaskCategoryRepo.SaveItem(newCategory);

            Categories.Add(newCategory);
            return new GenericResponse<TaskCategory>();
        }

        public async Task<GenericResponse<MyTask>> AddTask()
        {
            int oldCategoryId = 0;
            var verify = App.TaskRepo.GetItem(x => x.Name.ToLower() == NewTask.Name.ToLower());
            if (verify.Result != null && verify.Result.Id != NewTask.Id)
            {
                return new GenericResponse<MyTask> { HasError = true, StatusMessage = $"Task {NewTask.Name} already exists." };
            }
            else if (verify.Result != null && verify.Result.Id == NewTask.Id && verify.Result.CategoryId != SelectedCategory.Id)
            {
                oldCategoryId = verify.Result.CategoryId;
            }

            NewTask.TaskColor = SelectedCategory.Color;
            NewTask.Category = SelectedCategory;
            NewTask.CategoryId = SelectedCategory.Id;

            var taskResult = await App.TaskRepo.SaveItem(NewTask);

            await UpdateCategoryInfo(NewTask.CategoryId);
            if (oldCategoryId > 0) await UpdateCategoryInfo(oldCategoryId);

            return taskResult;
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

        private void RefreshCategories()
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
                UpdateCategorySelection();
                _canExecuteCommands = true;
            }
        }

        private void UpdateCategorySelection()
        {
            try
            {
                _canExecuteCommands = false;

                foreach (var category in Categories)
                {
                    category.IsSelected = SelectedCategory.Id == category.Id;
                }
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        public void RefreshCategory()
        {
            var categories = App.TaskCategoryRepo.GetItems();

            if (!categories.HasError)
            {
                Categories ??= new();

                Categories.Clear();
                foreach (var c in categories.Result)
                {
                    Categories.Add(c);
                }
            }
        }
    }
}
