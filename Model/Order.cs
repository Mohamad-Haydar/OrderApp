using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace OrderApp.Model
{
    public partial class Order : ObservableObject
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int UserId { get; set; }
        public bool IsLoaded { get; set; } = false;

        [ObservableProperty]
        float total;
        public DateTime DateToPick { get; set; }
        [ObservableProperty]
        ObservableCollection<ProductsInOrders> products = [];

        public bool IsValid(out string error)
        {
            if (DateToPick < DateTime.Today)
            {
                error = "Date must be today or in the future.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public void CalculateTotal()
        {
            Total = Products.Sum(item => item.Quantity * item.Product.Price);
        }

        public void UpdateTotal(float difference)
        {
            Total += difference;
        }

    }
}
