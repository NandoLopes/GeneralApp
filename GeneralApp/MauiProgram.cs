using CommunityToolkit.Maui;
using GeneralApp.Interfaces;
using GeneralApp.MVVM.Home;
using GeneralApp.MVVM.ViewModels;
using GeneralApp.MVVM.ViewModels.ColorPicker;
using GeneralApp.MVVM.ViewModels.TaskManager;
using GeneralApp.MVVM.Views.ColorPicker;
using GeneralApp.MVVM.Views.TaskManager;
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
		mauiAppBuilder.Services.AddSingleton<NewTaskViewModel>();
		mauiAppBuilder.Services.AddSingleton<ColorPickerViewModel>();
		return mauiAppBuilder;
	}

    #endregion

    #region Views

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<Home>();
		mauiAppBuilder.Services.AddSingleton<TaskerHomeView>();
		mauiAppBuilder.Services.AddSingleton<NewTaskView>();
		mauiAppBuilder.Services.AddSingleton<ColorPickerView>();
		return mauiAppBuilder;
	}

    #endregion
}
