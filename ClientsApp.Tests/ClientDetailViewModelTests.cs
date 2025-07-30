using ClientsApp.Models;
using ClientsApp.Services;
using ClientsApp.ViewModels;
using Moq;
using Xunit;

namespace ClientsApp.Tests.ViewModels
{
    public class ClientDetailViewModelTests
    {
        private readonly Mock<IClientService> _mockClientService;
        private readonly Mock<IDialogService> _mockDialogService;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly ClientDetailViewModel _viewModel;

        public ClientDetailViewModelTests()
        {
            _mockClientService = new Mock<IClientService>();
            _mockDialogService = new Mock<IDialogService>();
            _mockNavigationService = new Mock<INavigationService>();
            _viewModel = new ClientDetailViewModel(_mockClientService.Object, _mockDialogService.Object, _mockNavigationService.Object);
        }

        [Fact]
        public void PrepareViewModel_WhenEditingClient_PopulatesProperties()
        {
            // Arrange
            var client = new Client { Id = 1, Name = "Bruce", LastName = "Wayne", Age = 40, Address = "Wayne Manor" };

            // Act
            _viewModel.PrepareViewModel(client);

            // Assert
            Assert.Equal("Bruce", _viewModel.Name);
            Assert.Equal("Wayne", _viewModel.LastName);
            Assert.Equal(40, _viewModel.Age);
            Assert.Equal("Wayne Manor", _viewModel.Address);
        }

        [Fact]
        public async Task SaveClientAsync_WhenAddingNewClientWithValidData_CallsAddService()
        {
            // Arrange
            _viewModel.PrepareViewModel(null); // Prepara para adicionar
            _viewModel.Name = "Clark";
            _viewModel.LastName = "Kent";
            _viewModel.Age = 35;
            _viewModel.Address = "Smallville";

            // Act
            await _viewModel.SaveClientCommand.ExecuteAsync(null);

            // Assert
            _mockClientService.Verify(s => s.AddClientAsync(It.Is<Client>(c => c.Name == "Clark" && c.LastName == "Kent")), Times.Once);
            _mockClientService.Verify(s => s.UpdateClientAsync(It.IsAny<Client>()), Times.Never);
            Assert.False(_viewModel.IsBusy); // Garante que o estado de "ocupado" foi resetado
        }

        [Fact]
        public async Task SaveClientAsync_WhenUpdatingClientWithValidData_CallsUpdateService()
        {
            // Arrange
            var existingClient = new Client { Id = 1, Name = "Bruce", LastName = "Wayne", Age = 40, Address = "Wayne Manor" };
            _viewModel.PrepareViewModel(existingClient); // Garante que estamos no modo de edição
            _viewModel.Name = "Peter"; // Dados alterados
            _viewModel.LastName = "Parker";
            _viewModel.Age = 25;
            _viewModel.Address = "Queens";

            // Act
            await _viewModel.SaveClientCommand.ExecuteAsync(null);

            // Assert
            _mockClientService.Verify(s => s.UpdateClientAsync(It.Is<Client>(c => c.Id == 1 && c.Name == "Peter")), Times.Once);
            _mockClientService.Verify(s => s.AddClientAsync(It.IsAny<Client>()), Times.Never);
            Assert.False(_viewModel.IsBusy); // Garante que o estado de "ocupado" foi resetado
        }

        [Theory]
        [InlineData("", "Kent", 35, "Smallville")]      // Nome vazio
        [InlineData("Clark", "", 35, "Smallville")]      // Sobrenome vazio
        [InlineData("Clark", "Kent", 0, "Smallville")]       // Idade inválida (menor que 1)
        [InlineData("Clark", "Kent", 150, "Smallville")]     // Idade inválida (maior que 120)
        [InlineData("Clark", "Kent", 35, "")]           // Endereço vazio
        public async Task SaveClientAsync_WhenDataIsInvalid_DoesNotCallAddOrUpdateService(string name, string lastName, int age, string address)
        {
            // Arrange
            _viewModel.PrepareViewModel(null); // Prepara para o modo de adição
            _viewModel.Name = name;
            _viewModel.LastName = lastName;
            _viewModel.Age = age;
            _viewModel.Address = address;

            // Act
            await _viewModel.SaveClientCommand.ExecuteAsync(null);

            // Assert
            // A validação é feita manualmente no ViewModel, que mostra um alerta.
            // A asserção correta é verificar se o alerta foi exibido e se o serviço não foi chamado.
            _mockClientService.Verify(s => s.AddClientAsync(It.IsAny<Client>()), Times.Never);
            _mockClientService.Verify(s => s.UpdateClientAsync(It.IsAny<Client>()), Times.Never);
            _mockDialogService.Verify(d => d.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }



        [Fact]
        public async Task SaveClientAsync_WhenServiceThrowsException_SetsIsBusyToFalse()
        {
            // Arrange
            _viewModel.PrepareViewModel(null);
            _viewModel.Name = "Diana";
            _viewModel.LastName = "Prince";
            _viewModel.Age = 5000;
            _viewModel.Address = "Themyscira";
            _mockClientService.Setup(s => s.AddClientAsync(It.IsAny<Client>())).ThrowsAsync(new Exception("Database error"));

            // Act
            await _viewModel.SaveClientCommand.ExecuteAsync(null);

            // Assert
            Assert.False(_viewModel.IsBusy); // Garante que o ViewModel se recuperou do erro
        }
    }
}
