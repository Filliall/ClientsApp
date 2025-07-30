using ClientsApp.ViewModels;

namespace ClientsApp.Services
{
    public interface INavigationService
    {
        Task PushModalAsync<TViewModel>(object? parameter = null) where TViewModel : BaseViewModel;
        Task PopModalAsync();
    }
}