using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderApp.Model
{
    public partial class ProductsInOrders : ObservableObject
    {
        public int Id { get; set; }
        public Product Product { get; set; } = new Product();
        public int OrderId { get; set; }
        [ObservableProperty]
        int quantity;
    }
}
