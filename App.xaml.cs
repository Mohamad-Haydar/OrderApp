using OrderApp.Services;

namespace OrderApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            AdoDatabaseService.Init();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new LoginShell());
        }
    }
}