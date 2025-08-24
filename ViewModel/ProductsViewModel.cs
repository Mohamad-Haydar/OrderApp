using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OrderApp.ViewModel
{
    public partial class ProductsViewModel : BaseViewModel
    {
        private readonly PopupService _popupService;
        private readonly ProductsServices _productsServices;
        
        private bool _isDataLoaded = false;
        private bool _isSearchMode = false;
        private int _currentPage = 1;
        private const int PageSize = 7;
        private bool _hasMore = true;

        [ObservableProperty]
        ObservableCollection<Product> products;

        [ObservableProperty]
        ObservableCollection<string> suggestions = new();

        [ObservableProperty]
        string searchText;
        [ObservableProperty]
        bool suggestionsVisible;

        public ProductsViewModel()
        {
            Products = [];
            _popupService = ServiceHelper.Resolve<PopupService>();
            _productsServices = ServiceHelper.Resolve<ProductsServices>();
        }

        [RelayCommand]
        async Task AddProductAsync()
        {
            try
            {
                await _popupService.ShowAddProductPopupAsync(Products);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while opening the add product popup. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task GoToProductDetailsAsync()
        {
            try
            {
                // Implement navigation or details logic here if needed
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while navigating to product details. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task SelectImageAsync(Product product)
        {
            try
            {
                // Pick an image
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select an image"
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();

                    var appDataPath = FileSystem.AppDataDirectory;
                    var imagesFolder = Path.Combine(appDataPath, "images");

                    if (!Directory.Exists(imagesFolder))
                        Directory.CreateDirectory(imagesFolder);

                    string extension = Path.GetExtension(result.FileName);
                    string newFileName = $"{product.Id}{extension}";
                    string filePath = Path.Combine(imagesFolder, newFileName);

                    if (File.Exists(filePath))
                        File.Delete(filePath);

                    using var newFileStream = File.Create(filePath);
                    await stream.CopyToAsync(newFileStream);

                    product.ImageUrl = filePath;

                    // Update database with new image URL
                    await _productsServices.UpdateProductImage(product);

                }
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while selecting an image. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task AddProductToStockAsync(Product product)
        {
            try
            {
                await _productsServices.UpdateProductStock(-95,product.Id);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while updating product stock. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task EditProductAsync(Product product)
        {
            try
            {
                await _popupService.ShowEditProductPopupAsync(Products, product);
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while updating product stock. Please try again.", "OK");
            }
        }

        [RelayCommand]
        public async Task InitAsync()
        {
            if (_isDataLoaded) return;
            try
            {
                await Task.Yield();
                IsBusy = true;
                _isDataLoaded = true;
                await Task.Delay(500);
                List<Product> productsres;

                productsres = await _productsServices.GetProductsPagination(_currentPage, PageSize);
                

                foreach (var item in productsres)
                    Products.Add(item);


                if (productsres.Count < PageSize)
                    _hasMore = false;
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading products of the order. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async Task LoadProductsAsync()
        {
            try
            {
                //await Task.Yield();
                IsBusy = true;
                //await Task.Delay(500);
                List<Product> productsres;

                productsres = await _productsServices.GetProductsPagination(_currentPage, PageSize);

                Products.Clear();
                foreach (var item in productsres)
                    Products.Add(item);


                if (productsres.Count < PageSize)
                    _hasMore = false;
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while loading products of the order. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        [RelayCommand]
        public async Task LoadMoreProductsAsync()
        {
            if (IsBusy || !_hasMore) return;
            IsBusy = true;

            _currentPage++;
            List<Product> items;

            items = await _productsServices.GetProductsPagination(_currentPage, PageSize);
            
            foreach (var item in items)
                Products.Add(item);

            if (items.Count < PageSize)
                _hasMore = false;

            IsBusy = false;
        }


        [RelayCommand]
        public async Task SelectSuggestionAsync(string suggestion)
        {
            SuggestionsVisible = false;
            var items = await _productsServices.SearchProductsPagination(suggestion, 1, PageSize);

            Products = new ObservableCollection<Product>(items);

            // reset pagination state
            _currentPage = 1;
            _hasMore = items.Count >= PageSize;
        }

        partial void OnSearchTextChanged(string value)
        {
            _ = LoadSuggestionsAsync(value);
        }

        private async Task LoadSuggestionsAsync(string prefix)
        {
            await Task.Yield();
            if (string.IsNullOrWhiteSpace(prefix))
            {
                SuggestionsVisible = false;
                Suggestions.Clear();
                _currentPage = 1;
                await LoadProductsAsync();
                return;
            }
            SuggestionsVisible = true;
            var results = await _productsServices.GetProductSuggestionsAsync(prefix);

            Suggestions.Clear();
            foreach (var s in results)
                Suggestions.Add(s);
        }
    }
}
