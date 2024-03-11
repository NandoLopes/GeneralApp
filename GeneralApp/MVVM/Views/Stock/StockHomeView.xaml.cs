using GeneralApp.Abstractions.Interfaces;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels.Stock;
using GeneralApp.Services;

namespace GeneralApp.MVVM.Views.Stock;

public partial class StockHomeView : ContentPage
{
    private readonly StockHomeViewModel _viewModel;
    private readonly DialogService _dialogService;
    private readonly INavigationService _navigationService;

    public StockHomeView(StockHomeViewModel viewModel,
                         DialogService dialogService,
                         INavigationService navigationService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _dialogService = dialogService;
        _navigationService = navigationService;
        BindingContext = _viewModel;
    }

    private async void AddProductClicked(object sender, EventArgs e)
    {
        await _navigationService.NavigateToPage<NewItemView>();
    }

    private async void AddCategoryClicked(object sender, EventArgs e)
    {
        string category =
            await DisplayPromptAsync("New Category",
            "Write the new category name",
            maxLength: 15,
            keyboard: Keyboard.Text);

        if (category == null)
        {
            return;
        }
        else if (string.IsNullOrEmpty(category.Trim()))
        {
            await DisplayAlert("Error", "The new Category needs a name!", "Ok");
            return;
        }

        //TODO - Chose color.

        var color = System.Drawing.Color.FromArgb(category.GetHashCode());

        var result = await _viewModel.AddCategory(new ProductCategory
        {
            Name = category.Trim(),
            Color = Color.FromRgb(color.R, color.G, color.B).ToHex()
        });

        if (result.HasError)
        {
            await DisplayAlert("Error", result.StatusMessage, "Ok");
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Category created!");
        }
    }
}