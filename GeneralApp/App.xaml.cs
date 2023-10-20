using GeneralApp.MVVM.Home;
using GeneralApp.MVVM.Models;
using GeneralApp.Repositories;

namespace GeneralApp;

public partial class App : Application
{
    public static BaseRepository<MyTask> TaskRepo { get; private set; }
    public static BaseRepository<Product> ProductRepo { get; private set; }
    public static BaseRepository<TaskCategory> TaskCategoryRepo { get; private set; }
    public static BaseRepository<ProductCategory> ProductCategoryRepo { get; private set; }
    public static BaseRepository<TaskProduct> TaskProductRepo { get; private set; }

    public App(BaseRepository<MyTask> taskRepo,
               BaseRepository<TaskCategory> taskCategoryRepo,
               BaseRepository<ProductCategory> productCategoryRepo,
               BaseRepository<Product> productRepo,
               BaseRepository<TaskProduct> taskProductRepo,
               Home home)
    {
        InitializeComponent();

        TaskRepo = taskRepo;
        TaskCategoryRepo = taskCategoryRepo;
        ProductCategoryRepo = productCategoryRepo;
        ProductRepo = productRepo;
        TaskProductRepo = taskProductRepo;

        MainPage = new NavigationPage(home);  //<--- Official
                                              //MainPage = new NavigationPage(new NewTaskView());
    }
}
