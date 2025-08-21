using OrderApp.Model;
using System.Collections.ObjectModel;

namespace OrderApp.Services
{
    public class ProductDataService
    {
        private ObservableCollection<Product> _cachedProducts;
        private DateTime _lastRefreshTime = DateTime.MinValue;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
        private readonly ProductsServices _productsServices;

        public ProductDataService(ProductsServices productsServices)
        {
            _productsServices = productsServices;
            _cachedProducts = new ObservableCollection<Product>();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(bool forceRefresh = false)
        {
            if (forceRefresh || _cachedProducts.Count == 0 || DateTime.Now - _lastRefreshTime > _cacheExpiration)
            {
                var products = await _productsServices.GetProducts();
                _cachedProducts.Clear();
                foreach (var product in products)
                {
                    _cachedProducts.Add(product);
                }
                _lastRefreshTime = DateTime.Now;
            }

            return _cachedProducts;
        }

        public IEnumerable<Product> FilterProducts(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return _cachedProducts;

            return _cachedProducts.Where(p =>
                p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }
    }
}