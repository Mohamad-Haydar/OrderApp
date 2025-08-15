using CommunityToolkit.Mvvm.Input;
using OrderApp.Helper;
using OrderApp.Services;

namespace OrderApp.ViewModel
{
    public partial class ShellViewModel : BaseViewModel
    {

        public ShellViewModel(string language)
        {
            Language = language;
        }

        [RelayCommand]
        async Task SwitchLanguageAsync()
        {
            try
            {
                await base.SwitchLanguageAsync();
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while switching language. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task SwitchTheme()
        {
            try
            {
                await base.SelectTheme();
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while switching theme. Please try again.", "OK");
            }
        }

        [RelayCommand]
        async Task LogoutAsync()
        {
            try
            {
                Preferences.Remove("UserId");
                Preferences.Remove("Username");
                SecureStorage.Remove("SavedEmail");
                SecureStorage.Remove("SavedPassword");
                Application.Current.Windows[0].Page = new LoginShell();
                var themeService = ServiceHelper.Resolve<ThemeService>();
                themeService.SetTheme(Preferences.Get("AppTheme", "white"));
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Error", "An unexpected error occurred while logging out. Please try again.", "OK");
            }
        }


    }
}