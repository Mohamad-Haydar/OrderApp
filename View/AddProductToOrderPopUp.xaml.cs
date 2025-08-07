using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddProductToOrderPopUp : Popup
{
	public AddProductToOrderPopUp(ObservableCollection<Product> products, Order order)
	{
		InitializeComponent();
        this.BindingContext = new AddProductToOrderPopupViewModel(products, order, this, new ProductsServices() , new OrderServices(new ProductsServices(), new ProductInOrdersServices()));
    }
}