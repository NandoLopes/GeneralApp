using CommunityToolkit.Mvvm.Input;
using GeneralApp.MVVM.Models;
using PropertyChanged;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    class TaskerHomeViewModel
    {
        public bool _isRefreshing;
        public bool IsRefreshing 
        {
            get => _isRefreshing; 
            set => _isRefreshing = value;
        }

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<MyTask> Tasks { get; set; }

        public IAsyncRelayCommand PullToRefreshCommand { get; set; }


        public TaskerHomeViewModel()
        {
            FillData();
            Tasks.CollectionChanged += Tasks_CollectionChanged;

            PullToRefreshCommand = new AsyncRelayCommand(ExecPullToRefreshCommand, CanExecPullToRefreshCommand);
        }

        private bool _canExecPullToRefreshCommand = true;
        private bool CanExecPullToRefreshCommand() => _canExecPullToRefreshCommand;
        private async Task ExecPullToRefreshCommand()
        {
            if (_canExecPullToRefreshCommand)
            {
                _canExecPullToRefreshCommand = false;

                UpdateData();
                //Delay to check functionality
                await Task.Delay(2000);
                IsRefreshing = false;
                _canExecPullToRefreshCommand = true;
            }
        }

        private void Tasks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void FillData()
        {
            Categories = new ObservableCollection<Category>
            {
                new Category
                {
                    Id = 1,
                    Name = "Study",
                    Color = "#ff2b20"
                },
                new Category
                {
                    Id = 2,
                    Name = "House",
                    Color = "#e9c6b8"
                },
                new Category
                {
                    Id = 3,
                    Name = "Relationship",
                    Color = "#ff3fc7"
                },
                new Category
                {
                    Id = 4,
                    Name = "Animals",
                    Color = "#fcf500"
                }
            };

            Tasks = new ObservableCollection<MyTask> {
                new MyTask
                {
                    Id = 1,
                    Name = "Study C#",
                    Completed = true,
                    CategoryId = 1,
                },
                new MyTask
                {
                    Id = 2,
                    Name = "Clean the house",
                    Completed = false,
                    CategoryId = 2,
                },
                new MyTask
                {
                    Id = 3,
                    Name = "Love my partner (Daily)",
                    Completed = true,
                    CategoryId = 3,
                },
                new MyTask
                {
                    Id = 4,
                    Name = "Feed the cats",
                    Completed = false,
                    CategoryId = 4,
                },
                new MyTask
                {
                    Id = 5,
                    Name = "Groceries",
                    Completed = false,
                    CategoryId = 2
                }
            };

            UpdateData();
        }

        public void UpdateData()
        {
            foreach (var category in Categories)
            {
                var tasks = Tasks.Where(t => t.CategoryId == category.Id);
                var completed = tasks.Where(t => t.Completed == true);
                var notCompleted = tasks.Where(t => t.Completed == false);

                category.PendingTasks = notCompleted.Count();
                category.Percentage = (float)completed.Count()/(float)tasks.Count();
            }

            foreach (var task in Tasks)
            {
                task.TaskColor = Categories.Where(x => x.Id == task.CategoryId).Select(c => c.Color).FirstOrDefault();
            }
        }
    }
}
