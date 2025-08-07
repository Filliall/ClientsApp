using ClientsApp.Views;

namespace ClientsApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(BrownianMotionPage), typeof(BrownianMotionPage));
        Routing.RegisterRoute(nameof(ClientDetailPage), typeof(ClientDetailPage));

    }
}
