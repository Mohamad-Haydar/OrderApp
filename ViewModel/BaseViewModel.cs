using CommunityToolkit.Mvvm.ComponentModel;
using OrderApp.Helper;
using OrderApp.Resources.Strings;
using OrderApp.Services;

namespace OrderApp.ViewModel
{
    public partial class BaseViewModel : ObservableObject
    {
        public LocalizationService _localization;

        private readonly ThemeService _themeService;
        public BaseViewModel()
        {
            _localization = ServiceHelper.Resolve<LocalizationService>();
            Language = Preferences.Get("AppLanguage", "en");
            

            _themeService = ServiceHelper.Resolve<ThemeService>();
            _themeService.SetTheme(Preferences.Get("AppTheme", "white"));
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;
        [ObservableProperty]
        string title;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEnglish))]
        [NotifyPropertyChangedFor(nameof(AppFlowDirection))]
        private string language;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDark))]
        public string theme;

        public bool IsEnglish => Language == "en";
        public bool IsDark => Theme == "dark";
        public bool IsNotBusy => !IsBusy;
        public FlowDirection AppFlowDirection => IsEnglish ? FlowDirection.LeftToRight : FlowDirection.RightToLeft;

        public async Task SwitchLanguageAsync()
        {
            var newLang = Preferences.Get("AppLanguage", "en") == "en" ? "ar" : "en";
            Preferences.Set("AppLanguage", newLang);
            _localization.SetLanguage(newLang);
            Language = newLang;
        }

        public virtual async Task SelectTheme()
        {
            var newTheme = Preferences.Get("AppTheme", "white") == "white" ? "dark" : "white";
            Theme = newTheme;
            _themeService.SetTheme(newTheme);
        }

    }

}
