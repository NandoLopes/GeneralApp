using GeneralApp.MVVM.Models;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels
{
    public class HomeViewModel
    {
        public ObservableCollection<AppFunction> FunctionsList { get; set; }
        public HomeViewModel()
        {
            FillData();
        }

        private void FillData()
        {
            FunctionsList = new ObservableCollection<AppFunction>
            {
                new AppFunction{ Id = 1, Name = "Task Manager" },
                new AppFunction { Id = 2, Name = "Calculator" },
                new AppFunction { Id = 3, Name = "Not Implemented" },
                new AppFunction { Id = 4, Name = "To be done" }
            };
        }
    }
}
