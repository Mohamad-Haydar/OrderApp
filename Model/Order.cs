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
    }
}
