using CommunityToolkit.Mvvm.Input;
using GeneralApp.Abstractions.Interfaces;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Models.Helpers;
using GeneralApp.MVVM.Views.Stock;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels.Stock
{
    public partial class StockHomeViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public ObservableCollection<ProductCategory> Categories { get; set; }
        public ObservableCollection<object> SelectedCategories { get; set; } //.NET MAUI bug won't update SelectedItems if not in an "object" type list.
        public ObservableCollection<object> SelectedProducts { get; set; } //.NET MAUI bug won't update SelectedItems if not in an "object" type list.
        public ObservableCollection<StockItem> StockItems { get; set; }

        public bool VisibleClearCategory { get; set; }
        public bool VisibleClearProduct { get; set; }
        public bool VisibleEditCategory { get; set; }
        public bool VisibleEditProduct { get; set; }

        public IAsyncRelayCommand PullToRefreshCommand { get; set; }

        public StockHomeViewModel(INavigationService navigationService)
        {
            Categories = new();
            SelectedCategories = new();
            SelectedProducts = new();
            StockItems = new();

            PullToRefreshCommand = new AsyncRelayCommand(ExecPullToRefreshCommand, CanExecPullToRefreshCommand);
            _navigationService = navigationService;
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
            try
            {
                _canExecuteCommands = false;

                SelectedCategories = new();
                SelectedProducts = new();
                VisibleClearCategory = false;
                VisibleEditCategory = false;
                VisibleEditProduct = false;
                FillData();
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
        private void CategorySelectionChanged()
        {
            try
            {
                _canExecuteCommands = false;

                ClearProductSelection();
                UpdateCategorySelection();
                RefreshStock();
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        [RelayCommand]
        private async void ClearCategorySelection()
        {
            try
            {
                _canExecuteCommands = false;

                SelectedCategories.Clear();
                VisibleClearCategory = false;
                await FillData();
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
        private void ProductSelectionChanged()
        {
            try
            {
                _canExecuteCommands = false;
                UpdateProductSelection();
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        [RelayCommand]
        private void ClearProductSelection()
        {
            try
            {
                _canExecuteCommands = false;

                SelectedProducts.Clear();
                UpdateProductSelection();
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        private void UpdateProductSelection()
        {
            try
            {
                _canExecuteCommands = false;

                VisibleClearProduct = SelectedProducts.Any();
                VisibleEditProduct = SelectedProducts.Count == 1;

                foreach (var task in StockItems)
                {
                    task.IsSelected = SelectedProducts.Cast<StockItem>().Any(y => y.Id == task.Id);
                }
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        [RelayCommand]
        private async void ProductEdit(string isEdit)
        {
            await _navigationService.NavigateToPage<NewItemView>(
                (isEdit == "edit" && SelectedProducts?.Count == 1) ? SelectedProducts.FirstOrDefault() : null
            );
        }

        public List<(StockItem, GenericResponse<StockItem>)> DeleteProducts()
        {
            List<(StockItem, GenericResponse<StockItem>)> errorProduct = new();

            foreach (StockItem product in SelectedProducts.Cast<StockItem>())
            {
                var result = App.StockRepo.DeleteItem(product, true);

                if (result.HasError) errorProduct.Add((product, result));
            }

            return errorProduct;
        }

        public List<(ProductCategory, GenericResponse<ProductCategory>)> DeleteCategories()
        {
            List<(ProductCategory, GenericResponse<ProductCategory>)> errorCategory = new();

            foreach (ProductCategory category in SelectedCategories.Cast<ProductCategory>())
            {
                var result = App.ProductCategoryRepo.DeleteItem(category, true);

                if (result.HasError) errorCategory.Add((category, result));
            }

            return errorCategory;
        }

        private Task FillData()
        {
            RefreshCategories();
            SelectedCategories.Clear();
            VisibleClearCategory = false;
            UpdateCategorySelection();
            SelectedProducts.Clear();
            RefreshStock();

            return Task.CompletedTask;
        }

        public void RefreshCategories()
        {
            try
            {
                _canExecuteCommands = false;

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
            finally
            {
                UpdateCategorySelection();
                _canExecuteCommands = true;
            }
        }

        public void RefreshStock()
        {
            try
            {
                _canExecuteCommands = false;

                var selectedCategories = SelectedCategories.Cast<ProductCategory>().ToList();
                var items = App.StockRepo.GetItemsWithChildrenPredicate(x => selectedCategories.Any(y => y.Id == x.ProductCategory?.Id) || selectedCategories.Count == 0);

                if (items.HasError || items.Result.Count == 0)
                {
                    StockItems = new();
                    return;
                }

                StockItems.Clear();
                foreach (var item in items.Result)
                {
                    StockItems.Add(item);
                }
            }
            finally
            {
                UpdateProductSelection();
                _canExecuteCommands = true;
            }
        }

        private void UpdateCategorySelection()
        {
            try
            {
                _canExecuteCommands = false;

                VisibleClearCategory = SelectedCategories.Any();
                VisibleEditCategory = SelectedCategories.Count == 1;

                foreach (var category in Categories)
                {
                    category.IsSelected = SelectedCategories.Cast<ProductCategory>().Any(y => y.Id == category.Id);
                }
            }
            finally
            {
                _canExecuteCommands = true;
            }
        }

        public async Task<GenericResponse<ProductCategory>> AddCategory(ProductCategory newCategory)
        {
            var verify = App.ProductCategoryRepo.GetItem(x => (x.Name.ToLower() == newCategory.Name.ToLower()) &&
                                                              (x.Id != newCategory.Id));
            if (verify.Result != null) return new GenericResponse<ProductCategory> { HasError = true, StatusMessage = $"Category {newCategory.Name} already exists." };

            var result = await App.ProductCategoryRepo.SaveItem(newCategory);

            SelectedCategories.Clear();
            RefreshCategories();
            return result;
        }
    }
}
