using CommunityToolkit.Mvvm.ComponentModel;

namespace OrderApp.Model
{
    public partial class Order : ObservableObject
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public int UserId { get; set; }

        [ObservableProperty]
        float total;
        public DateTime DateToPick { get; set; }
        public List<Product> Products { get; set; } = [];


        public bool IsValid(out string error)
        {
            if (DateToPick < DateTime.Today)
            {
                error = "Date must be today or in the future.";
                return false;
            }

            if (Products.Count == 0)
            {
                error = "Order must contain at least one product.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public void CalculateTotal()
        {
            Total = Products.Sum(p => p.Price * p.Quantity);
        }

    }
}
