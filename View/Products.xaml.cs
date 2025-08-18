using OrderApp.ViewModel;

namespace OrderApp.View;

public partial class Products : ContentPage
{
    private ProductsViewModel _productsViewModel;
    public Products(ProductsViewModel viewModel, ProductsViewModel productsViewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _productsViewModel = productsViewModel;
    }
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _productsViewModel.LoadProducts();
    }
}