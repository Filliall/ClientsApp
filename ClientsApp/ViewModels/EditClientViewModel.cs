using ClientsApp.Models;
using ClientsApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientsApp.ViewModels
{
    [QueryProperty(nameof(Client), "ClientEdit")]
    public partial class EditClientViewModel : BaseViewModel
    {
        private Client client;
        private readonly IClientService _clientService;
        private readonly IDialogService _dialogService;
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

        public Client? Client { set => SetClientForEditing(value); }

        public EditClientViewModel(Client client, IClientService clientService, IDialogService dialogService)
        {
            Client = client;
            SaveClientAsyncCommand = new AsyncRelayCommand(SaveClientAsync!);
            CancelAsyncCommand = new AsyncRelayCommand(GoBackAsync!);
            _clientService = clientService;
            _dialogService = dialogService;
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task GoBackAsync()
        {
            await Shell.Current.Navigation.PopModalAsync();
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async Task SaveClientAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(LastName))
            {
                await _dialogService.DisplayAlert("Erro de Validação", "Nome e Sobrenome são obrigatórios.", "OK");
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
                {
                    await _clientService.AddClientAsync(clientToSave);
                }
                else
                {
                    await _clientService.UpdateClientAsync(clientToSave);
                }

                // Navega de volta para a página anterior
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


        private void SetClientForEditing(Client? client)
        {
            if (client != null)
            {
                Id = client.Id;
                Name = client.Name;
                LastName = client.LastName;
                Address = client.Address;
                Age = client.Age;
                Title = "Editar Cliente";
            }
        }
    }
}
