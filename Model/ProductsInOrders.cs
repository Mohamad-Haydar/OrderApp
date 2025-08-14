using CommunityToolkit.Mvvm.ComponentModel;

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
    }
}
