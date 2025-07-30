using ClientsApp.ViewModels;

namespace ClientsApp.Services
{
    public class NavigationService : INavigationService
    {


        public Task PopModalAsync()
        {
            // Acessa a pilha modal da página principal da aplicação para garantir consistência.
            if (Application.Current?.MainPage != null &&
                Application.Current.MainPage.Navigation.ModalStack.Any())
            {
                return Application.Current.MainPage.Navigation.PopModalAsync(true);
            }
            return Task.CompletedTask;
        }

        public Task PushModalAsync<TViewModel>(object? parameter = null) where TViewModel : BaseViewModel
        {
            return Application.Current.MainPage.Navigation.PushModalAsync((Page)parameter, true);

        }
    }
}