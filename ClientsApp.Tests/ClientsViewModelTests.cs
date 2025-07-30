using ClientsApp.Models;
using ClientsApp.Services;
using ClientsApp.ViewModels;
using Moq;
using Xunit;

namespace ClientsApp.Tests
{
    public class ClientsViewModelTests
    {
        private readonly Mock<IClientService> _mockClientService;
        private readonly Mock<IDialogService> _mockDialogService;
        private readonly Mock<INavigationService> _mockNavigationService;
        private readonly ClientsViewModel _viewModel;

        public ClientsViewModelTests()
        {
            _mockClientService = new Mock<IClientService>();
            _mockDialogService = new Mock<IDialogService>();
            _mockNavigationService = new Mock<INavigationService>();
            _viewModel = new ClientsViewModel(_mockClientService.Object, _mockDialogService.Object, _mockNavigationService.Object);
        }

        [Fact]
        public async Task LoadClientsAsync_ShouldLoadClientsFromService()
        {
            // Arrange
            var clients = new List<Client>
            {
                new Client { Id = 1, Name = "Test", LastName = "User" }
            };
            _mockClientService.Setup(s => s.GetClientsAsync()).ReturnsAsync(clients);

            // Act
            await _viewModel.LoadClientsAsync();

            // Assert
            Assert.Single(_viewModel.Clients);
            Assert.Equal("Test", _viewModel.Clients[0].Name);
            _mockClientService.Verify(s => s.GetClientsAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteClientCommand_WhenNotConfirmed_ShouldNotDeleteClient()
        {
            // Arrange
            var client = new Client { Id = 1, Name = "Test", LastName = "User" };
            _viewModel.Clients.Add(client);
            _mockDialogService.Setup(d => d.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), "Sim", "Não")).ReturnsAsync(false);

            // Act
            await _viewModel.DeleteClientCommand.ExecuteAsync(client);

            // Assert
            _mockClientService.Verify(s => s.DeleteClientAsync(It.IsAny<int>()), Times.Never);
            Assert.Single(_viewModel.Clients);
        }

        [Fact]
        public async Task DeleteClientCommand_WhenConfirmed_ShouldDeleteClientAndRemoveFromList()
        {
            // Arrange
            var client = new Client { Id = 1, Name = "Test", LastName = "User" };
            _viewModel.Clients.Add(client);
            _mockDialogService.Setup(d => d.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), "Sim", "Não")).ReturnsAsync(true);

            // Act
            await _viewModel.DeleteClientCommand.ExecuteAsync(client);

            // Assert
            _mockClientService.Verify(s => s.DeleteClientAsync(client.Id), Times.Once);
            Assert.Empty(_viewModel.Clients);
        }

    }
}