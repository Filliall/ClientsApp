using ClientsApp.Services;
using ClientsApp.ViewModels;
using ClientsApp.Views;
using Microsoft.Extensions.Logging;

namespace ClientsApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("IMFellEnglish-Regular.ttf", "FellEnglish");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Serviços
        builder.Services.AddSingleton<IClientService, ClientService>();
        builder.Services.AddSingleton<IDialogService, DialogService>();
        builder.Services.AddSingleton<IAppNavigationService, AppNavigationService>();

        // ViewModels
        builder.Services.AddTransient<ClientsViewModel>();
        builder.Services.AddTransient<AddClientViewModel>();

        // Views (Páginas)
        builder.Services.AddTransient<ClientsPage>();
        builder.Services.AddTransient<AddClientPage>();

        return builder.Build();
    }
}
