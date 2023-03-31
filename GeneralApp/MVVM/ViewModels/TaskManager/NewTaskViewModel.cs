using GeneralApp.MVVM.Models;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels.TaskManager
{
    public class NewTaskViewModel
    {
        public string Task { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<MyTask> Tasks { get; set; }
    }
}
