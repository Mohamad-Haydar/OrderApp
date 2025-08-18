using OrderApp.Services;

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
            return new Window(new LoginShell());
        }

        public void SetRootPage(Page page)
        {
            // This replaces the root page of the main window
            MainPage = page;
        }
    }
}