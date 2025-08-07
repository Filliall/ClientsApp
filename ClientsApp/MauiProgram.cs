using ClientsApp.Services;
using ClientsApp.ViewModels;
using ClientsApp.Views;
using Microcharts.Maui;
using Microsoft.Extensions.Logging;

namespace ClientsApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("IMFellEnglish-Regular.ttf", "FellEnglish");
                fonts.AddFont("Orbitron-Regular.ttf", "CyberpunkFont");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Serviços
        builder.Services.AddSingleton<IClientService, ClientService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IBrownianService, BrownianService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<IMainThreadInvoker, RealMainThreadInvoker>();

        // ViewModels
        builder.Services.AddTransient<ClientsViewModel>();
        builder.Services.AddTransient<ClientDetailViewModel>();
        builder.Services.AddTransient<BrownianMotionViewModel>();

        // Views (Páginas)
        builder.Services.AddTransient<BrownianMotionPage>();
        builder.Services.AddTransient<ClientsPage>();
        builder.Services.AddTransient<ClientDetailPage>();



        return builder.Build();
    }
}
