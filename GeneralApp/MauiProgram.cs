using CommunityToolkit.Maui;
using GeneralApp.Abstractions.Interfaces;
using GeneralApp.Interfaces;
using GeneralApp.MVVM.Home;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels;
using GeneralApp.MVVM.ViewModels.ColorPicker;
using GeneralApp.MVVM.ViewModels.TaskManager;
using GeneralApp.MVVM.Views.ColorPicker;
using GeneralApp.MVVM.Views.TaskManager;
using GeneralApp.Repositories;
using GeneralApp.Services;
using Microsoft.Extensions.Logging;

namespace GeneralApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .RegisterViewModels()
            .RegisterViews()
            .RegisterServices()
            .RegisterRepositories()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    #region Services

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<INavigationService, NavigationService>();
        mauiAppBuilder.Services.AddSingleton<DialogService>();
        return mauiAppBuilder;
    }

    #endregion

    #region ViewModels

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<HomeViewModel>();
        mauiAppBuilder.Services.AddSingleton<TaskerHomeViewModel>();
        mauiAppBuilder.Services.AddScoped<NewTaskViewModel>();
        mauiAppBuilder.Services.AddSingleton<ColorPickerViewModel>();
        return mauiAppBuilder;
    }

    #endregion

    #region Views

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<Home>();
        mauiAppBuilder.Services.AddSingleton<TaskerHomeView>();
        mauiAppBuilder.Services.AddScoped<NewTaskView>();
        mauiAppBuilder.Services.AddSingleton<ColorPickerView>();
        return mauiAppBuilder;
    }

    #endregion

    #region Repositories

    public static MauiAppBuilder RegisterRepositories(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<BaseRepository<MyTask>>();
        mauiAppBuilder.Services.AddSingleton<BaseRepository<Product>>();
        mauiAppBuilder.Services.AddSingleton<BaseRepository<ProductCategory>>();
        mauiAppBuilder.Services.AddSingleton<BaseRepository<TaskCategory>>();
        mauiAppBuilder.Services.AddSingleton<BaseRepository<TaskProduct>>();
        mauiAppBuilder.Services.AddSingleton<BaseRepository<Stock>>();
        return mauiAppBuilder;
    }

    #endregion
}
