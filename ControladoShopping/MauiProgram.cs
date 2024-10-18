using Microsoft.Extensions.Logging;
using Camera.MAUI;
using Microsoft.Maui.Hosting;
using CommunityToolkit.Maui;
using Syncfusion.Maui.Core.Hosting;
using ControladoShopping.Views;
using ControladoShopping.ViewModels;
using ControladoShopping.Data.Local;

namespace ControladoShopping
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .UseMauiCameraView()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.ConfigureSyncfusionCore();

            builder.Services.AddSingleton(new VolumeScannerRepository("ColetorSQLite.db3"));
            builder.Services.AddTransient<Principal>();
            builder.Services.AddTransient<Scanner>();
            builder.Services.AddTransient<PrincipalViewModel>();

            return builder.Build();
        }
    }
}
