using CommunityToolkit.Mvvm.Input;
using GeneralApp.MVVM.Models;
using GeneralApp.MVVM.Models.Helpers;
using System.Collections.ObjectModel;

namespace GeneralApp.MVVM.ViewModels.Stock
{
    public partial class StockHomeViewModel : ViewModelBase
    {
        public ObservableCollection<ProductCategory> Categories { get; set; }
        public ObservableCollection<object> SelectedCategories { get; set; } //.NET MAUI bug won't update SelectedItems if not in an "object" type list.
        public ObservableCollection<StockItem> StockItems { get; set; }

        public bool VisibleClearCategory { get; set; }

        public IAsyncRelayCommand PullToRefreshCommand { get; set; }

        public StockHomeViewModel()
        {
            Categories = new();
            SelectedCategories = new();
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
            _canExecuteCommands = false;

            SelectedCategories = new();
            VisibleClearCategory = false;
            FillData();

            _canExecuteCommands = true;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
        private void CategorySelectionChanged()
        {
            _canExecuteCommands = false;

            VisibleClearCategory = SelectedCategories.Any();
            UpdateCategorySelection();
            RefreshStock();

            _canExecuteCommands = true;
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

        private Task FillData()
        {
            RefreshCategories();
            SelectedCategories.Clear();
            VisibleClearCategory = false;
            UpdateCategorySelection();
            RefreshStock();

            return Task.CompletedTask;
        }

        private void RefreshCategories()
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

            _canExecuteCommands = true;
        }

        private void RefreshStock()
        {
            _canExecuteCommands = false;

            var selectedCategories = SelectedCategories.Cast<ProductCategory>().ToList();
            var items = App.StockRepo.GetItemsWithChildrenPredicate(x => selectedCategories.Any(y => y.Id == x.ProductCategory.Id) || selectedCategories.Count == 0);

            if (items.HasError || items.Result.Count == 0)
            {
                StockItems = new();
            }

            StockItems.Clear();
            foreach (var item in items.Result)
            {
                StockItems.Add(item);
            }

            _canExecuteCommands = true;
        }

        private void UpdateCategorySelection()
        {
            _canExecuteCommands = false;

            foreach (var category in Categories)
            {
                category.IsSelected = SelectedCategories.Cast<ProductCategory>().Any(y => y.Id == category.Id);
            }

            _canExecuteCommands = true;
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
