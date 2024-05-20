using GeneralApp.MVVM.Views.Stock;
using GeneralApp.MVVM.Views.TaskManager;

namespace GeneralApp.Shared
{
    public static class Routes
    {
        public static string TaskerHome => nameof(TaskerHomeView);
        public static string NewTask => nameof(NewTaskView);
        public static string StockHome => nameof(StockHomeView);
        public static string NewItem => nameof(NewItemView);
    }
}
