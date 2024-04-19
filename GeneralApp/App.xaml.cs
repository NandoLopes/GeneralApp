using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Views.TaskManager;
using GeneralApp.Repositories;

namespace GeneralApp;

public partial class App : Application
{
    public static BaseRepository<MyTask> TaskRepo { get; private set; }
    public static BaseRepository<StockItem> StockRepo { get; private set; }
    public static BaseRepository<TaskCategory> TaskCategoryRepo { get; private set; }
    public static BaseRepository<ProductCategory> ProductCategoryRepo { get; private set; }
    public static BaseRepository<TaskItem> TaskProductRepo { get; private set; }

    public App(BaseRepository<MyTask> taskRepo,
               BaseRepository<TaskCategory> taskCategoryRepo,
               BaseRepository<ProductCategory> productCategoryRepo,
               BaseRepository<StockItem> stockRepo,
               BaseRepository<TaskItem> taskProductRepo,
               TaskerHomeView home)
    {
        InitializeComponent();

        TaskRepo = taskRepo;
        TaskCategoryRepo = taskCategoryRepo;
        ProductCategoryRepo = productCategoryRepo;
        StockRepo = stockRepo;
        TaskProductRepo = taskProductRepo;

        MainPage = new NavigationPage(home);
    }
}
