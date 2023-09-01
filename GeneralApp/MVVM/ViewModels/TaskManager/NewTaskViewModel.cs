using GeneralApp.MVVM.Models;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels.TaskManager
{
    public partial class NewTaskViewModel : ViewModelBase
    {
        public string NewTask { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public Category SelectedCategory { get; set; }
        public ObservableCollection<MyTask> Tasks { get; set; }

        public override Task OnNavigatingTo(object? parameter)
        {
            if (parameter == null)
                return Task.CompletedTask;

            if (parameter is Tuple<ObservableCollection<MyTask>, ObservableCollection<Category>>)
            {
                (Tasks, Categories) = parameter as Tuple<ObservableCollection<MyTask>, ObservableCollection<Category>>;
            }

            return Task.CompletedTask;
        }
    }
}
