using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Models.Helpers;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels.TaskManager
{
    public partial class NewTaskViewModel : ViewModelBase
    {
        public string NewTask { get; set; }
        public ObservableCollection<TaskCategory> Categories { get; set; }
        public ObservableCollection<MyTask> Tasks { get; set; }

        public override Task OnNavigatingTo(object? parameter)
        {
            RefreshCategory();

            if (parameter == null)
                return Task.CompletedTask;

            if (parameter is Tuple<ObservableCollection<MyTask>, ObservableCollection<TaskCategory>>)
            {
                (Tasks, Categories) = parameter as Tuple<ObservableCollection<MyTask>, ObservableCollection<TaskCategory>>;
            } else
            {
                Categories = new();
                Tasks = new();
            }

            return Task.CompletedTask;
        }

        public async Task<GenericResponse<TaskCategory>> AddCategory(TaskCategory newCategory)
        {
            var verify = App.TaskCategoryRepo.GetItem(x => x.Name.ToLower() == newCategory.Name.ToLower());
            if (verify.Result != null) return new GenericResponse<TaskCategory> { HasError = true, StatusMessage = $"Category {newCategory.Name} already exists."};

            await App.TaskCategoryRepo.SaveItem(newCategory);

            Categories.Add(newCategory);
            return new GenericResponse<TaskCategory>();
        }

        public async Task<GenericResponse<MyTask>> AddTask(MyTask task, TaskCategory taskCategory)
        {
            task.TaskColor = taskCategory.Color;

            var taskResult = await App.TaskRepo.SaveItem(task);

            var category = App.TaskCategoryRepo.GetItemWithChildren(task.CategoryId);

            if (category.HasError) return new GenericResponse<MyTask> { HasError = true, StatusMessage = category.StatusMessage }; ;

            category.Result.PendingTasks += 1;

            if (category.Result.MyTasks.Count <= 1)
            {
                category.Result.Percentage = 0;
                await App.TaskCategoryRepo.SaveItem(category.Result);
            } 

            var categoryTasks = category.Result.MyTasks ??= new();
            var completedTasks = category.Result.MyTasks.Where(x => x.Completed).Count();

            category.Result.Percentage = (float)completedTasks / (float)categoryTasks.Count;

            await App.TaskCategoryRepo.SaveItem(category.Result);

            return taskResult;
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
