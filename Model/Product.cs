using CommunityToolkit.Mvvm.ComponentModel;

namespace OrderApp.Model
{
    public partial class Product : ObservableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        [ObservableProperty]
        int quantity;
        public string ImageUrl { get; set; }
    }
}
