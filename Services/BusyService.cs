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
        private readonly SemaphoreSlim _popupSemaphore = new(1, 1);
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
                if (e.PropertyName != nameof(IsBusy) || Shell.Current == null)
                    return;

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
                else if (_isPopupOpen)
                {
                    _isPopupOpen = false;
                    await _loadingPopup.CloseAsync();
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
