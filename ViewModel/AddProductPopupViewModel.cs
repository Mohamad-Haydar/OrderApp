using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OrderApp.Exceptions;
using OrderApp.Helper;
using OrderApp.Model;
using OrderApp.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace OrderApp.ViewModel
{
    public partial class AddProductPopupViewModel : ObservableObject
    {
        [ObservableProperty]
        Product product;
        [ObservableProperty]
        string title;

        private readonly ObservableCollection<Product> _products;
        private ProductsServices _productsServices;
        private int mode = 0; // 0 for add, 1 for update

        public AddProductPopupViewModel(ObservableCollection<Product> products, Product p = null, int mode = 0)
        {
            Product = p?.Clone() ?? new Product();
            Title = mode == 0 ? "Add New Product" : $"Edit Product # {Product.Id}"; 
            _products = products;
            _productsServices = ServiceHelper.Resolve<ProductsServices>();
            this.mode = mode; // Set the mode (add or update)
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

                    //// Update database with new image URL
                    //await _productsServices.UpdateProductImage(product);

                }
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while selecting an image. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task SaveProduct()
        {
            if(mode == 0)
            {
                await AddProductAsync();
            }
            else
            {
                await UpdateProductAsync();
            }
        }

        private async Task AddProductAsync()
        {
            try
            {
                await _productsServices.AddProductAsync(Product.Name, Product.Description, Product.Price, Product.Quantity, Product.ImageUrl);
                _products.Add(new Product() { Name = Product.Name, Description = Product.Description, Price = Product.Price, Quantity = Product.Quantity, ImageUrl = Product.ImageUrl});
                Product = new Product();

                await Shell.Current.DisplayAlert("Success", "Product added successfully", "OK");
                // Signal to close the popup
                WeakReferenceMessenger.Default.Send(new ClosePopupMessage());
            }
            catch (ValidationException vex)
            {
                await Shell.Current.DisplayAlert("Validation Error", vex.Message, "OK");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while adding the product. Please try again.", "OK");
            }
        }

        private async Task UpdateProductAsync()
        {
            try
            {
                await _productsServices.ChangeProductInfo(Product);
                // find the product in the collection
                var existingProduct = _products.FirstOrDefault(x => x.Id == Product.Id);
                if (existingProduct != null)
                {
                    existingProduct.ImageUrl = Product.ImageUrl;
                    // update other properties if needed
                    existingProduct.Name = Product.Name;
                    existingProduct.Price = Product.Price;
                }

                WeakReferenceMessenger.Default.Send(new ClosePopupMessage());
            }
            catch (Exception ex)
            {
                // Example: show error to user
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");

                // Or log it
                Debug.WriteLine(ex);
            }
        }
    }
}
