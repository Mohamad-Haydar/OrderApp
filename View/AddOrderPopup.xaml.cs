using CommunityToolkit.Maui.Views;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddOrderPopup : Popup
{
	public AddOrderPopup(ObservableCollection<Order> orders, ObservableCollection<Client> clients)
	{
		InitializeComponent();
        this.BindingContext = new AddOrderPopupViewModel(orders, clients, this,new OrderServices(new ProductsServices(), new ProductInOrdersServices()));
    }
}