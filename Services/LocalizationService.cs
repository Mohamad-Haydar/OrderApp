using OrderApp.Resources.Strings;
using System.ComponentModel;
using System.Globalization;

namespace OrderApp.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        private CultureInfo? _currentCulture;
        
        public event PropertyChangedEventHandler? PropertyChanged;

        public LocalizationService()
        {
            var lang = Preferences.Get("AppLanguage", "en");
            SetLanguage(lang);
        }

        public static LocalizationService Instance { get; } = new();
        public object this[string key] => AppResources.ResourceManager.GetString(key, AppResources.Culture) ?? key;
        

        public void SetLanguage(string languageCode)
        {
            _currentCulture = new CultureInfo(languageCode);
            CultureInfo.CurrentCulture = _currentCulture;
            CultureInfo.CurrentUICulture = _currentCulture;
            AppResources.Culture = _currentCulture;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
