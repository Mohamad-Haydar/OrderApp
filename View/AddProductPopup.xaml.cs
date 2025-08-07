using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Model;
using OrderApp.Services;
using OrderApp.ViewModel;
using System.Collections.ObjectModel;

namespace OrderApp.View;

public partial class AddProductPopup : Popup
{
    public AddProductPopup(ObservableCollection<Product> products)
    {
        InitializeComponent();
        this.BindingContext = new AddProductPopupViewModel(products, this, new ProductsServices());
    }
}