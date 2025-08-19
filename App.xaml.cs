using OrderApp.Services;
using OrderApp.ViewModel;

namespace OrderApp
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            AdoDatabaseService.Init();
            Services = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Use synchronous call here (bad to block, but acceptable at startup)
            var email = SecureStorage.GetAsync("SavedEmail").Result;
            var password = SecureStorage.GetAsync("SavedPassword").Result;

            Page startPage;

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password))
            {
                // User has logged in before -> go to AppShell directly
                startPage = new AppShell();
            }
            else
            {
                // No saved credentials -> go to login
                startPage = new LoginShell();
            }

            return new Window(startPage);
             //return new Window(new LoginShell());
        }

        public void SetRootPage(Page page)
        {
            // This replaces the root page of the main window
            MainPage = page;
        }
    }
}