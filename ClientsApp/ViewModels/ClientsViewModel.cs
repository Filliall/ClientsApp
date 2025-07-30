using ClientsApp.Models;
using ClientsApp.Services;
using ClientsApp.Views;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ClientsApp.ViewModels
{
    public partial class ClientsViewModel : BaseViewModel
    {
        private readonly IClientService _clientService;
        private readonly IDialogService _dialogService;
        private readonly IAppNavigationService _navigationService;
        public IAsyncRelayCommand GoToAddClientPageAsyncCommand { get; }
        public IAsyncRelayCommand<Client> GoToEditClientPageAsyncCommand { get; }
        public IAsyncRelayCommand<Client> DeleteClientAsyncCommand { get; }

        public ObservableCollection<Client> Clients { get; } = new();


        public ClientsViewModel(IClientService clientService, IDialogService dialogService, IAppNavigationService navigationService)
        {
            _clientService = clientService;
            _dialogService = dialogService;
            _navigationService = navigationService;

            GoToAddClientPageAsyncCommand = new AsyncRelayCommand(GoToAddClientPageAsync);
            GoToEditClientPageAsyncCommand = new AsyncRelayCommand<Client>(GoToEditClientPageAsync!);
            DeleteClientAsyncCommand = new AsyncRelayCommand<Client>(DeleteClientAsync!);
        }


        [RelayCommand]
        public async Task LoadClientsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var clients = await _clientService.GetClientsAsync();
                Clients.Clear();
                foreach (var client in clients)
                {
                    Clients.Add(client);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task GoToAddClientPageAsync()
        {
            var addViewModel = new AddClientViewModel(_clientService, _dialogService, _navigationService);
            var addPage = new AddClientPage { BindingContext = addViewModel };
            await Shell.Current.Navigation.PushModalAsync(addPage, true);

        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task GoToEditClientPageAsync(Client client)
        {
            if (client == null)
                return;

            var editViewModel = new EditClientViewModel(client, _clientService, _dialogService);
            var editPage = new EditClientPage { BindingContext = editViewModel };
            await Shell.Current.Navigation.PushModalAsync(editPage, true);
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task DeleteClientAsync(Client client)
        {
            if (client == null)
                return;

            bool confirmed = await _dialogService.DisplayAlert(
                "Confirmar Exclusão",
                $"Tem certeza que deseja excluir {client.Name} {client.LastName}?",
                "Sim", "Não");

            if (!confirmed)
                return;

            if (IsBusy) return;

            try
            {
                IsBusy = true;
                await _clientService.DeleteClientAsync(client.Id);
                Clients.Remove(client);
            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Erro", $"Falha ao excluir o cliente: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}