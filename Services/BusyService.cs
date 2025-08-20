using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls.Shapes;
using OrderApp.Components;
using OrderApp.Helper;

namespace OrderApp.Services
{
    public partial class BusyService : ObservableObject
    {
        private LoadingPopup? _loadingPopup;
        private bool _isPopupOpen = false;
        public static BusyService Instance { get; } = new();

        public BusyService()
        {
            _loadingPopup = ServiceHelper.Resolve<LoadingPopup>();
            this.PropertyChanged += OnBusyChanged;
        }

        [ObservableProperty]
        private bool isBusy;

        async void OnBusyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName != nameof(IsBusy))
                    return;

                if (Shell.Current == null)
                    return; // not ready yet

                if (Instance.IsBusy)
                {
                    _isPopupOpen = true;
                    await Shell.Current.ShowPopupAsync(_loadingPopup, new PopupOptions
                    {
                        Shape = new RoundRectangle
                        {
                            BackgroundColor = Colors.Transparent,

                        },
                        Shadow = null
                    });
                }
                else
                {
                    if (_isPopupOpen)
                    {
                        await _loadingPopup.CloseAsync();
                        _isPopupOpen = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while updating the order. Please try again.", "OK");

                throw;
            }

        }
    }
}
