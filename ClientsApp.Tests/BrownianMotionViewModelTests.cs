using ClientsApp.Models;
using ClientsApp.Services;
using ClientsApp.ViewModels;
using Moq;
using Xunit;

namespace ClientsApp.Tests
{
    public class BrownianMotionViewModelTests
    {
        private readonly Mock<IDialogService> _mockDialogService;
        private readonly Mock<IBrownianService> _mockBrownianService;
        private readonly Mock<IMainThreadInvoker> _mockMainThreadInvoker;
        private readonly BrownianMotionViewModel _viewModel;

        // No xUnit, o construtor é usado para a configuração que antes estava no [SetUp]
        public BrownianMotionViewModelTests()
        {
            // Configuração inicial que roda antes de cada teste
            _mockDialogService = new Mock<IDialogService>();
            _mockBrownianService = new Mock<IBrownianService>();
            _mockMainThreadInvoker = new Mock<IMainThreadInvoker>();

            // Configura o mock do invoker para executar a ação imediatamente no mesmo thread.
            _mockMainThreadInvoker.Setup(m => m.BeginInvokeOnMainThread(It.IsAny<Action>()))
                                  .Callback<Action>(action => action());

            _viewModel = new BrownianMotionViewModel(
                _mockDialogService.Object,
                _mockBrownianService.Object,
                _mockMainThreadInvoker.Object);
        }
        [Fact]
        public async Task StartSimulationAsync_WithValidInputs_CreatesMultipleDataSets()
        {
            // Arrange (Organizar)
            _viewModel.NumberOfSimulations = 3;
            // Configura o serviço mock para retornar um resultado válido
            _mockBrownianService.Setup(s => s.GenerateSimulation(It.IsAny<BrownianDataModel>(), It.IsAny<Random>()))
                                .Returns(new double[] { 100, 101, 102 });

            // Act (Agir)
            await _viewModel.StartSimulationCommand.ExecuteAsync(null);

            // Assert (Verificar)
            // Verifica se o serviço de simulação foi chamado 3 vezes
            _mockBrownianService.Verify(s => s.GenerateSimulation(It.IsAny<BrownianDataModel>(), It.IsAny<Random>()), Times.Exactly(3));
            // Verifica se os dados da simulação foram gerados
            Assert.NotNull(_viewModel.LastSimulationData);
            Assert.Equal(3, _viewModel.LastSimulationData.Count);
            // Verifica se o indicador de "ocupado" foi desligado
            Assert.False(_viewModel.IsBusy);
        }

        [Fact]
        public async Task StartSimulationAsync_WithInvalidInputs_ShowsAlertAndDoesNotSimulate()
        {
            // Arrange (Organizar)
            _viewModel.InitialPrice = "abc"; // Entrada inválida

            // Act (Agir)
            await _viewModel.StartSimulationCommand.ExecuteAsync(null);

            // Assert (Verificar)
            // Verifica se um alerta de erro foi exibido
            _mockDialogService.Verify(d => d.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            // Verifica se o serviço de simulação NUNCA foi chamado
            _mockBrownianService.Verify(s => s.GenerateSimulation(It.IsAny<BrownianDataModel>(), It.IsAny<Random>()), Times.Never);
            // Verifica se nenhum gráfico foi criado
            Assert.Null(_viewModel.LastSimulationData);
        }
    }
}
