using OrderApp.Resources.Strings;
using System.ComponentModel;
using System.Globalization;

namespace OrderApp.Services
{
    public class LocalizationService
    {

        private readonly ResourceDictionary _english = new EnglishStrings();
        private readonly ResourceDictionary _arabic = new ArabicStrings();

        public LocalizationService()
        {
            var lang = Preferences.Get("AppLanguage", "en");
            SetLanguage(lang);
        }

        public void SetLanguage(string languageCode)
        {
            var appResources = Shell.Current.Resources;

            // Remove previous language dictionary
            var existing = appResources.MergedDictionaries.FirstOrDefault(d => d is EnglishStrings || d is ArabicStrings);
            if (existing != null)
                appResources.MergedDictionaries.Remove(existing);

            if (languageCode == "en")
                appResources.MergedDictionaries.Add(new EnglishStrings());
            else
                appResources.MergedDictionaries.Add(new ArabicStrings());
        }
    }
}
