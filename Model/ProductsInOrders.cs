using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace OrderApp.Model
{
    public partial class ProductsInOrders : ObservableObject
    {
        public int Id { get; set; }
        [ObservableProperty]
        Product product;
        public int OrderId { get; set; }
        [ObservableProperty]
        int quantity;

        private int _previousQuantity;
        private bool _isUpdatingQuantity = false;


        partial void OnQuantityChanging(int value)
        {
            if(product is null)
                return;
            _previousQuantity = Quantity;
        }

        partial void OnQuantityChanged(int value)
        {
            if (product is null || _isUpdatingQuantity)
                return;

            if(value == _previousQuantity)
            {
                Quantity = _previousQuantity;
                return;
            }
            if (value > product.Quantity)
            {
                _isUpdatingQuantity = true;

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.DisplayAlert("Error", "Not enough stock available", "OK");

                    // revert to previous value without triggering OnQuantityChanged again
                    Quantity = _previousQuantity;

                    _isUpdatingQuantity = false;
                });
            }
            else
            {
                int difference = quantity - _previousQuantity;
                product.Quantity -= difference;
            }
        }

        public bool IsValid(out string error)
        {
            if (Quantity < 0)
            {
                error = "Quantity cannot be negative.";
                return false;
            }

            if (Product == null)
            {
                error = "Product must be selected.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool ShouldBeDeleted()
        {
            return Quantity == 0;
        }
    }
}
