using ClientsApp.Models;
using ClientsApp.Services;
using ClientsApp.ViewModels;
using ClientsApp.Views;
using Moq;
using Xunit;

namespace ClientsApp.Tests
{
    public class ClientsViewModelTests
    {
        private readonly Mock<IClientService> _mockClientService;
        private readonly Mock<IDialogService> _mockDialogService;
        private readonly Mock<IAppNavigationService> _mockNavigationService;
        private readonly ClientsViewModel _viewModel;

        public ClientsViewModelTests()
        {
            _mockClientService = new Mock<IClientService>();
            _mockDialogService = new Mock<IDialogService>();
            _mockNavigationService = new Mock<IAppNavigationService>();
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

        [Fact]
        public async Task GoToAddClientPageCommand_ShouldNavigate()
        {
            // Act
            await _viewModel.GoToAddClientPageCommand.ExecuteAsync(null);

            // Assert
            _mockNavigationService.Verify(n => n.GoToAsync("AddClientPage", true), Times.Once);
        }

        [Fact]
        public async Task GoToEditClientPageAsyncCommand_ShouldNavigateWithParameter()
        {
            // Arrange
            var client = new Client { Id = 1, Name = "Test", LastName = "User" };
            IDictionary<string, object> parameters = null;
            _mockNavigationService.Setup(n => n.GoToAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()))
                .Callback<string, bool, IDictionary<string, object>>((route, animate, p) => parameters = p)
                .Returns(Task.CompletedTask);

            // Act
            await _viewModel.GoToEditClientPageCommand.ExecuteAsync(client);

            // Assert
            _mockNavigationService.Verify(n => n.GoToAsync(nameof(AddClientPage), true, It.IsAny<IDictionary<string, object>>()), Times.Once);
            Assert.NotNull(parameters);
            Assert.True(parameters.ContainsKey("Client"));
            Assert.Equal(client, parameters["Client"]);
        }
    }
}