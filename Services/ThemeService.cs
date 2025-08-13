using OrderApp.Helper;
using OrderApp.Resources.Styles;

namespace OrderApp.Services
{
    public class ThemeService
    {
        public MyAppTheme? CurrentTheme { get; private set; }

        public ThemeService()
        {
            SetTheme(Preferences.Get("AppTheme", "white"));
        }

        public void SetTheme(string theme)
        {
            Preferences.Set("AppTheme", theme);

            if (theme == "dark")
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }

            //var appResources = Shell.Current.Resources;

            //appResources.MergedDictionaries.Clear();
            //if (theme == "dark")
            //    appResources.MergedDictionaries.Add(new DarkTheme());
            //else
            //    appResources.MergedDictionaries.Add(new LightTheme());
        }
    }
}
