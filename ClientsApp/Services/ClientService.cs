using ClientsApp.Data;
using ClientsApp.Models;
using SQLite;

namespace ClientsApp.Services
{
    public class ClientService : IClientService
    {
        private SQLiteAsyncConnection? _database;

        private async Task Init()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath);
            await _database.CreateTableAsync<Client>();

            // Adiciona dados de exemplo se a tabela estiver vazia
            if (await _database.Table<Client>().CountAsync() == 0)
            {
                await _database.InsertAsync(new Client { Name = "Maria", LastName = "Silva", Address = "Rua A, 123", Age = 30 });
                await _database.InsertAsync(new Client { Name = "João", LastName = "Santos", Address = "Rua B, 456", Age = 45 });
                await _database.InsertAsync(new Client { Name = "Ana", LastName = "Pereira", Address = "Av. C, 789", Age = 28 });
            }
        }

        public async Task AddClientAsync(Client client)
        {
            await Init();
            await _database!.InsertAsync(client);
        }

        public async Task DeleteClientAsync(int id)
        {
            await Init();
            await _database!.DeleteAsync<Client>(id);
        }

        public async Task<Client?> GetClientAsync(int id)
        {
            await Init();
            return await _database!.Table<Client>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Client>> GetClientsAsync()
        {
            await Init();
            return await _database!.Table<Client>().OrderBy(c => c.Name).ToListAsync();
        }

        public async Task UpdateClientAsync(Client client)
        {
            await Init();
            await _database!.UpdateAsync(client);
        }
    }
}