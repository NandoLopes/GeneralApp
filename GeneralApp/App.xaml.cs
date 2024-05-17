using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Views.TaskManager;
using GeneralApp.Repositories;

namespace GeneralApp;

public partial class App : Application
{
    public static TaskRepository TaskRepo { get; private set; }
    public static StockRepository StockRepo { get; private set; }
    public static BaseRepository<TaskCategory> TaskCategoryRepo { get; private set; }
    public static BaseRepository<ProductCategory> ProductCategoryRepo { get; private set; }
    public static BaseRepository<TaskItem> TaskProductRepo { get; private set; }

    public App(TaskRepository taskRepo,
               StockRepository stockRepo,
               BaseRepository<TaskCategory> taskCategoryRepo,
               BaseRepository<ProductCategory> productCategoryRepo,
               BaseRepository<TaskItem> taskProductRepo,
               TaskerHomeView home)
    {
        InitializeComponent();

        TaskRepo = taskRepo;
        StockRepo = stockRepo;
        TaskCategoryRepo = taskCategoryRepo;
        ProductCategoryRepo = productCategoryRepo;
        TaskProductRepo = taskProductRepo;

        MainPage = new NavigationPage(home);
    }
}
