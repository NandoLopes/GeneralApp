using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.ViewModels.Stock;
using GeneralApp.Services;

namespace GeneralApp.MVVM.Views.Stock;

public partial class NewItemView : ContentPage
{
    private readonly NewItemViewModel _newItemViewModel;
    private readonly DialogService _dialogService;

    public NewItemView(NewItemViewModel newItemViewModel,
                       DialogService dialogService)
	{
		InitializeComponent();
        _newItemViewModel = newItemViewModel;
        _dialogService = dialogService;
        BindingContext = _newItemViewModel;
    }

    private async void SaveItemClicked(object sender, EventArgs e)
    {
        var result = await _newItemViewModel.SaveButton();

        if (result.HasError)
        {
            await DisplayAlert("Error", result.StatusMessage, "Ok");
            return;
        }

        await _dialogService.SnackbarSuccessAsync("Item saved!");
        await Navigation.PopAsync();
    }

    private async void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;

        if (picker.SelectedItem == null || picker.SelectedIndex != 0)
        {
            _newItemViewModel.Validate();
            return;
        }

        string category =
            await DisplayPromptAsync("New Category",
            "Write the new category name",
            maxLength: 15,
            keyboard: Keyboard.Text);

        if (category == null)
        {
            _newItemViewModel.Validate();
            return;
        }
        else if (string.IsNullOrEmpty(category.Trim()))
        {
            await DisplayAlert("Error", "The new Category needs a name!", "Ok");
            picker.SelectedItem = null;
            _newItemViewModel.Validate();
            return;
        }

        //TODO - Chose color.
        var r = new Random();

        var result = await _newItemViewModel.AddCategory(new ProductCategory
        {
            Name = category.Trim(),
            Color = Color.FromRgb(
                r.Next(0, 255),
                r.Next(0, 255),
                r.Next(0, 255)).ToHex()
        });

        if (result.HasError)
        {
            await DisplayAlert("Error", result.StatusMessage, "Ok");
            _newItemViewModel.Validate();
        }
        else
        {
            await _dialogService.SnackbarSuccessAsync("Category created!");

            _newItemViewModel.NewStockItem.ProductCategory = _newItemViewModel.Categories.FirstOrDefault(x => x.Name == category);
            _newItemViewModel.Validate();
        }
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        _newItemViewModel.Validate();
    }

    private void DateCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        var checkBox = sender as CheckBox;

        if (!checkBox.IsChecked)
        {
            _newItemViewModel.NewStockItem.ExpirationDate = null;
        }
    }

    private async void Quantity_TextChanged(object sender, TextChangedEventArgs e)
    {
        var entry = sender as Entry;

        await Task.Delay(1);

        bool isInt = int.TryParse(entry.Text.Trim(), out int quantity);

        if (!isInt)
        {
            _newItemViewModel.NewStockItem.Quantity = 0;
            _newItemViewModel.Validate();
        } else
        {
            _newItemViewModel.NewStockItem.Quantity = quantity >= 0 ? quantity : 0;
            _newItemViewModel.Validate();
        }

    }
}