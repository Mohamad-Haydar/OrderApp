using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrderApp.Services;

namespace OrderApp.ViewModel
{
    public partial class ShellViewModel : BaseViewModel
    {

        public ShellViewModel(LocalizationService localizationService, string language, ThemeService themeService)
        : base(localizationService, themeService)
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
        }


    }
}