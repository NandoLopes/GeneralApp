using GeneralApp.MVVM.ViewModels;

namespace GeneralApp.MVVM.Home;

public partial class Home : ContentPage
{
    public Home(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}