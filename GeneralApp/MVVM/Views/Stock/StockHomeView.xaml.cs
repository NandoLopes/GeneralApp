using GeneralApp.Abstractions.Interfaces;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels.Stock;
using GeneralApp.Services;
using System.Text;

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
        var category = _viewModel.SelectedCategories.Count == 1 ? _viewModel.SelectedCategories[0] as ProductCategory : new();
        string categoryName =
            (await DisplayPromptAsync("New Category",
            "Write the new category name",
            maxLength: 15,
            keyboard: Keyboard.Text,
            initialValue: category.Name))?.Trim();

        if (categoryName == null)
        {
            return;
        }
        else if (string.IsNullOrEmpty(categoryName))
        {
            await DisplayAlert("Error", "The Category needs a name!", "Ok");
            return;
        }
        
        category.Name = categoryName;

        //TODO - Chose color.
        if(category.Id == 0)
        {
            var color = System.Drawing.Color.FromArgb(categoryName.GetHashCode());
            category.Color = Color.FromRgb(color.R, color.G, color.B).ToHex();
        }

        var result = await _viewModel.AddCategory(category);

        if (result.HasError)
        {
            await DisplayAlert("Error", result.StatusMessage, "Ok");
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Category created!");
        }
    }

    private async void DeleteProducts(object sender, EventArgs e)
    {
        var tasksToDeleteCount = _viewModel.SelectedProducts.Count;
        bool confirmDelete = await DisplayAlert("CONFIRM", $"Delete {tasksToDeleteCount} item(s)?", "Yes", "No");

        if (!confirmDelete) return;

        var errors = _viewModel.DeleteProducts();

        if (errors.Any())
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append("Error trying to delete item(s) with Id(s): ");
            foreach (var error in errors)
            {
                stringBuilder.Append($"{error.Item1.Id}, ");
            }
            stringBuilder.Append("sorry :(");

            await _dialogService.SnackbarErrorAsync(stringBuilder.ToString());
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Item(s) deleted!");
        }

        _viewModel.SelectedCategories.Clear();
        _viewModel.RefreshStock();
    }

    private async void DeleteCategories(object sender, EventArgs e)
    {
        var categoriesToDeleteCount = _viewModel.SelectedCategories.Count;
        bool confirmDelete = await DisplayAlert("CONFIRM", $"Delete {categoriesToDeleteCount} categories? This will also delete their items.", "Yes", "No");

        if (!confirmDelete) return;

        var errors = _viewModel.DeleteCategories();

        if (errors.Any())
        {
            StringBuilder stringBuilder = new();

            stringBuilder.Append("Error trying to delete categories with Ids: ");
            foreach (var error in errors)
            {
                stringBuilder.Append($"{error.Item1.Id}, ");
            }
            stringBuilder.Append("sorry :(");

            await _dialogService.SnackbarErrorAsync(stringBuilder.ToString());
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Categories deleted!");
        }

        _viewModel.SelectedCategories.Clear();
        _viewModel.RefreshCategories();
    }
}