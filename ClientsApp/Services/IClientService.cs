using ClientsApp.Models;

namespace ClientsApp.Services
{
    public interface IClientService
    {
        Task<List<Client>> GetClientsAsync();
        Task<Client?> GetClientAsync(int id);
        Task AddClientAsync(Client client);
        Task UpdateClientAsync(Client client);
        Task DeleteClientAsync(int id);
    }
}