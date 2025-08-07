using CommunityToolkit.Maui.Views;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddClientPopup : Popup
{
    public AddClientPopup(ObservableCollection<Client> clients)
    {
        InitializeComponent();
        this.BindingContext = new AddClientPopupViewModel(clients, this,new ClientServices());
    }
}