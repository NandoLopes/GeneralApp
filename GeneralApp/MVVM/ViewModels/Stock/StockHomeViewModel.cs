using CommunityToolkit.Mvvm.Input;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Models.Helpers;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels.Stock
{
    public partial class StockHomeViewModel : ViewModelBase
    {
        public ObservableCollection<ProductCategory> Categories { get; set; }
        public ObservableCollection<StockItem> StockItems { get; set; }

        public IAsyncRelayCommand PullToRefreshCommand { get; set; }

        public StockHomeViewModel()
        {
            Categories = new();
            StockItems = new();

            PullToRefreshCommand = new AsyncRelayCommand(ExecPullToRefreshCommand, CanExecPullToRefreshCommand);
        }

        private bool _canExecPullToRefreshCommand = true;

        private bool CanExecPullToRefreshCommand() => _canExecPullToRefreshCommand;
        private async Task ExecPullToRefreshCommand()
        {
            if (_canExecPullToRefreshCommand)
            {
                _canExecPullToRefreshCommand = false;

                await FillData();
                IsRefreshing = false;
                _canExecPullToRefreshCommand = true;
            }
        }

        [RelayCommand]
        private void Appearing()
        {
            FillData();
        }

        private Task FillData()
        {
            RefreshCategories();
            RefreshStock();

            return Task.CompletedTask;
        }

        private void RefreshCategories()
        {
            var categories = App.ProductCategoryRepo.GetItemsWithChildren();

            if (categories.HasError || categories.Result.Count == 0)
            {
                Categories = new();
                return;
            }

            Categories.Clear();
            foreach (var category in categories.Result)
            {
                Categories.Add(category);
            }
        }

        private void RefreshStock()
        {
            var stock = App.StockRepo.GetItemsWithChildren();

            if (stock.HasError || stock.Result.Count == 0)
            {
                StockItems = new();
                return;
            }

            StockItems.Clear();
            foreach (var item in stock.Result)
            {
                StockItems.Add(item);
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
    }
}
