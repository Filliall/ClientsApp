using ClientsApp.Views;

namespace ClientsApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(AddClientPage), typeof(AddClientPage));
    }
}
