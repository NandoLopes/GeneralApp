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
        public bool IsValid { get; set; }

        public NewTaskViewModel()
        {
            Categories = new();
            NewTask = new();

            IsValid = false;
        }

        public override Task OnNavigatingTo(object? parameter)
        {
            if (parameter is MyTask stockItem)
            {
                Title = "Edit Item";
                NewTask = stockItem;
            }
            else
            {
                Title = "New Item";
                NewTask = new();
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        private async void Appearing()
        {
            await FillData();

            if (NewTask.Id != 0)
            {
                SelectedCategory = Categories.FirstOrDefault(x => x.Id == NewTask.CategoryId);
            }

            Validate();
        }

        public async Task<GenericResponse<MyTask>> SaveButton()
        {
            var verify = App.TaskRepo.GetItem(x => (x.Name.ToLower() == NewTask.Name.ToLower()) &&
                                                    (x.Id != NewTask.Id));

            if (verify.Result != null) return new GenericResponse<MyTask> { HasError = true, StatusMessage = "There is another task with this name." };

            return await App.TaskRepo.SaveItemWithChildren(NewTask);
        }

        public async Task<GenericResponse<TaskCategory>> UpdateCategoryInfo(int categoryId)
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

        private Task FillData()
        {
            RefreshCategories();

            return Task.CompletedTask;
        }

        private void RefreshCategories()
        {
            var categories = App.TaskCategoryRepo.GetItemsWithChildren();

            if (categories.HasError || categories.Result.Count == 0)
            {
                Categories = new()
                {
                    new TaskCategory { Name = "Create Category" }
                };
                return;
            }

            Categories.Clear();
            Categories.Add(new TaskCategory { Name = "Create Category" });

            foreach (var category in categories.Result)
            {
                Categories.Add(category);
            }
        }

        public async Task<GenericResponse<TaskCategory>> AddCategory(TaskCategory newCategory)
        {
            var verify = App.TaskCategoryRepo.GetItem(x => x.Name.ToLower() == newCategory.Name.ToLower());
            if (verify.Result != null) return new GenericResponse<TaskCategory> { HasError = true, StatusMessage = $"Category {newCategory.Name} already exists." };

            await App.TaskCategoryRepo.SaveItem(newCategory);

            Categories.Add(newCategory);
            return new GenericResponse<TaskCategory>();
        }

        public void Validate()
        {
            IsValid = (!String.IsNullOrEmpty(NewTask.Category?.Name)
                    && !String.IsNullOrEmpty(NewTask.Name?.Trim()));
        }
    }
}
