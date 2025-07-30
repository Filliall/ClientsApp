using ClientsApp.Models;
using ClientsApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientsApp.ViewModels
{
    /// <summary>
    /// ViewModel unificado para adicionar e editar clientes.
    /// </summary>
    public partial class ClientDetailViewModel : BaseViewModel
    {
        private readonly IClientService _clientService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        public IAsyncRelayCommand SaveClientAsyncCommand { get; }
        public IAsyncRelayCommand CancelAsyncCommand { get; }

        [ObservableProperty]
        private int _id;
        [ObservableProperty]
        private string _name = string.Empty;
        [ObservableProperty]
        private string _lastName = string.Empty;
        [ObservableProperty]
        private string _address = string.Empty;
        [ObservableProperty]
        private int _age;
        [ObservableProperty]
        private string _title = "Adicionar Cliente";

        public ClientDetailViewModel(IClientService clientService, IDialogService dialogService, INavigationService navigationService)
        {
            _clientService = clientService;
            _dialogService = dialogService;
            _navigationService = navigationService;

            SaveClientAsyncCommand = new AsyncRelayCommand(SaveClientAsync);
            CancelAsyncCommand = new AsyncRelayCommand(GoBackAsync);
        }

        /// <summary>
        /// Prepara o ViewModel para adicionar um novo cliente ou editar um existente.
        /// </summary>
        public void PrepareViewModel(Client? client)
        {
            if (client != null && client.Id != 0)
            {
                Id = client.Id;
                Name = client.Name;
                LastName = client.LastName;
                Address = client.Address;
                Age = client.Age;
                Title = "Editar Cliente";
            }
            else
            {
                // Reset for new client
                Id = 0;
                Name = string.Empty;
                LastName = string.Empty;
                Address = string.Empty;
                Age = 0;
                Title = "Adicionar Cliente";
            }
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task GoBackAsync()
        {
            await _navigationService.PopModalAsync();
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task SaveClientAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Address) || Age <= 0 || Age > 120)
            {
                await _dialogService.DisplayAlert("Erro de Validação", "Por favor, preencha todos os campos corretamente.", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var clientToSave = new Client
                {
                    Id = this.Id,
                    Name = this.Name,
                    LastName = this.LastName,
                    Address = this.Address,
                    Age = this.Age
                };

                if (clientToSave.Id == 0)
                    await _clientService.AddClientAsync(clientToSave);
                else
                    await _clientService.UpdateClientAsync(clientToSave);

                await GoBackAsync();
            }
            catch (System.Exception ex)
            {
                await _dialogService.DisplayAlert("Erro", $"Falha ao salvar o cliente: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}