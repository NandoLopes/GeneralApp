using CommunityToolkit.Mvvm.Input;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Models.Helpers;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels.Stock
{
    public partial class NewItemViewModel : ViewModelBase
    {
        public ObservableCollection<ProductCategory> Categories { get; set; }
        public ProductCategory SelectedCategory { get; set; }
        public StockItem NewStockItem { get; set; }
        public string Quantity { get; set; }
        public bool IsValid { get; set; }

        public NewItemViewModel()
        {
            Categories = new();
            NewStockItem = new();

            IsValid = false;
        }

        public override Task OnNavigatingTo(object? parameter)
        {
            if (parameter is StockItem stockItem)
            {
                Title = "Edit Item";
                NewStockItem = stockItem;
                Quantity = NewStockItem.Quantity.ToString();
            }
            else
            {
                Title = "New Item";
                NewStockItem = new();
                Quantity = string.Empty;
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        private async void Appearing()
        {
            await FillData();

            if (NewStockItem.Id != 0)
            {
                SelectedCategory = Categories.FirstOrDefault(x => x.Id == NewStockItem.ProductCategoryId);
            }

            Validate();
        }

        public async Task<GenericResponse<StockItem>> SaveButton()
        {
            var verify = App.StockRepo.GetItem(x => (x.Product.ToLower() == NewStockItem.Product.ToLower()) &&
                                                    (x.Id != NewStockItem.Id));

            if (verify.Result != null) return new GenericResponse<StockItem> { HasError = true, StatusMessage = "There is another item with this name." };

            return await App.StockRepo.SaveItemWithChildren(NewStockItem);
        }

        private Task FillData()
        {
            RefreshCategories();

            return Task.CompletedTask;
        }

        private void RefreshCategories()
        {
            var categories = App.ProductCategoryRepo.GetItemsWithChildren();

            if (categories.HasError || categories.Result.Count == 0)
            {
                Categories = new()
                {
                    new ProductCategory { Name = "Create Category" }
                };
                return;
            }

            Categories.Clear();
            Categories.Add(new ProductCategory { Name = "Create Category" });

            foreach (var category in categories.Result)
            {
                Categories.Add(category);
            }
        }

        public async Task<GenericResponse<ProductCategory>> AddCategory(ProductCategory newCategory)
        {
            var verify = App.ProductCategoryRepo.GetItem(x => x.Name.ToLower() == newCategory.Name.ToLower());
            if (verify.Result != null) return new GenericResponse<ProductCategory> { HasError = true, StatusMessage = $"Category {newCategory.Name} already exists." };

            await App.ProductCategoryRepo.SaveItem(newCategory);

            Categories.Add(newCategory);
            return new GenericResponse<ProductCategory>();
        }

        public void Validate()
        {
            IsValid = (!String.IsNullOrEmpty(NewStockItem.ProductCategory?.Name) 
                    && !String.IsNullOrEmpty(NewStockItem.Product?.Trim())
                    && NewStockItem.Quantity > 0);
        }
    }
}
