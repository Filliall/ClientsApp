using ClientsApp.ViewModels;

namespace ClientsApp.Views;

public partial class AddClientPage : ContentPage
{
    public AddClientPage(AddClientViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}