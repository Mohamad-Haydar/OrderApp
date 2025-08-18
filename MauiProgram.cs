using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using OrderApp.Helper;
using OrderApp.Services;
using OrderApp.ViewModel;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Plugin.Firebase.CloudMessaging;
using Plugin.LocalNotification;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Core.Hosting;
using Microsoft.Maui.Hosting;




#if IOS
using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

namespace OrderApp
{
    public static class MauiProgram
    {
        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
#if IOS
        events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) => {
            CrossFirebase.Initialize();
            FirebaseCloudMessagingImplementation.Initialize();
            return false;
        }));
#elif ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) =>
         CrossFirebase.Initialize(activity)));
#endif
            });

            return builder;
        }

     

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .RegisterFirebaseServices()
                .UseLocalNotification()
                .ConfigureSyncfusionCore()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IFingerprint>(CrossFingerprint.Current);

            // view models
            builder.Services.AddSingleton<ClientsViewModel>();
            builder.Services.AddSingleton<ProductsViewModel>();
            builder.Services.AddSingleton<OrdersViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();

            // popup view models
            builder.Services.AddTransient<OrderDetailsViewModel>();
            builder.Services.AddTransient<AddClientPopupViewModel>();
            builder.Services.AddTransient<AddProductPopupViewModel>();
            builder.Services.AddTransient<AddOrderPopupViewModel>();
            builder.Services.AddTransient<AddEventPopUpViewModel>();
            builder.Services.AddTransient<EventsViewModel>();
            builder.Services.AddTransient<Popup>();

            // services and helpers
            builder.Services.AddSingleton<Helpers>();
            builder.Services.AddSingleton<ClientServices>();
            builder.Services.AddSingleton<PopupService>();
            builder.Services.AddSingleton<ProductsServices>();
            builder.Services.AddSingleton<LocalizationService>();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<OrderServices>();
            builder.Services.AddSingleton<LoginServices>();
            builder.Services.AddSingleton<ProductInOrdersServices>();
            builder.Services.AddSingleton<EventsServices>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
