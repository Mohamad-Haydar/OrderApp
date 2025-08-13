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

        public ObservableCollection<Product> Products { get;  }

        public ProductsViewModel()
        {
            Products = [];
            _popupService = ServiceHelper.Resolve<PopupService>();
            _productsServices = ServiceHelper.Resolve<ProductsServices>();
        }

        [RelayCommand]
        async Task AddProductAsync()
        {
            await _popupService.ShowAddProductPopupAsync(Products);
        }

        [RelayCommand]
        async Task GoToProductDetailsAsync()
        {

        }

        public async Task LoadProducts()
        {
            try
            {
                Products.Clear();
                var res = await _productsServices.GetProducts();

                foreach (var product in res)
                {
                    {
                        Products.Add(new Product
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Description = product.Description,
                            Price = product.Price,
                            Quantity = product.Quantity,
                            ImageUrl = product.ImageUrl
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: { ex.Message}");
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
            catch (Exception ex)
            {
                // Handle exceptions (user canceled, permissions, etc)
                Debug.WriteLine($"Image pick error: {ex.Message}");
            }
        }

        [RelayCommand]
        async Task AddProductToStockAsync(Product product)
        {
            await _productsServices.UpdateProductStock(-95,product.Id);
        }
    }
}
