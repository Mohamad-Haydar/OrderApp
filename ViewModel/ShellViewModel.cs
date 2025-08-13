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
           await base.SwitchLanguageAsync();
        }

        [RelayCommand]
        async Task SwitchTheme()
        {
            await base.SelectTheme();
        }

        [RelayCommand]
        async Task LogoutAsync()
        {
            Preferences.Remove("UserId");
            Preferences.Remove("Username");
            SecureStorage.Remove("SavedEmail");
            SecureStorage.Remove("SavedPassword");
            Application.Current.Windows[0].Page = new LoginShell();

            var themeService = ServiceHelper.Resolve<ThemeService>();
            themeService.SetTheme(Preferences.Get("AppTheme", "white"));
        }


    }
}