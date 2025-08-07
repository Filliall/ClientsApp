using ClientsApp.Models;
using ClientsApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientsApp.ViewModels
{
    public partial class BrownianMotionViewModel : BaseViewModel
    {

        [ObservableProperty]
        private string initialPrice = "100";

        [ObservableProperty]
        private string sigma = "20";

        [ObservableProperty]
        private string mean = "1";

        [ObservableProperty]
        private double numDays = 252;

        [ObservableProperty]
        private double numberOfSimulations = 3;


        partial void OnNumberOfSimulationsChanged(double value)
        {
            double roundedValue = Math.Round(value);
            if (NumberOfSimulations != roundedValue)
            {
                NumberOfSimulations = roundedValue;
            }
        }

        partial void OnNumDaysChanged(double value)
        {
            double roundedValue = Math.Round(value);
            if (NumDays != roundedValue)
            {
                NumDays = roundedValue;
            }
        }

        [ObservableProperty]
        private ColorOptionModel selectedColor;

        public List<ColorOptionModel> AvailableColors { get; }

        [ObservableProperty]
        private bool isLineFilled = true;

        [ObservableProperty]
        private List<double[]> lastSimulationData;

        partial void OnLastSimulationDataChanged(List<double[]> value)
        {
            ClearSimulationCommand.NotifyCanExecuteChanged();
        }

        private readonly IDialogService _dialogService;
        private readonly IBrownianService _brownianService;
        private readonly IMainThreadInvoker _mainThreadInvoker;
        private readonly Random _random = new(); // Um único gerador para semear os outros


        public BrownianMotionViewModel(IDialogService dialogService, IBrownianService brownianService, IMainThreadInvoker mainThreadInvoker)
        {
            _dialogService = dialogService;
            _brownianService = brownianService;
            _mainThreadInvoker = mainThreadInvoker;

            AvailableColors = new List<ColorOptionModel>
            {
                new ColorOptionModel { Name = "Cyber Cyan", HexValue = "#00FFFF" },
                new ColorOptionModel { Name = "Hacker Green", HexValue = "#00FF00" },
                new ColorOptionModel { Name = "Hot Pink", HexValue = "#FF69B4" },
                new ColorOptionModel { Name = "Electric Yellow", HexValue = "#FFFF00" },
                new ColorOptionModel { Name = "Synthwave Magenta", HexValue = "#FF00FF" },
                new ColorOptionModel { Name = "Data Orange", HexValue = "#FFA500" }
            };
            SelectedColor = AvailableColors.First();
        }

        [RelayCommand]
        private async Task StartSimulationAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Validação e conversão das entradas
                if (!TryParseInputs(out double s0, out double vol, out double mu))
                {
                    await _dialogService.DisplayAlert("Entrada Inválida", "Por favor, verifique se os valores de preço, volatilidade e drift são números válidos.", "OK");
                    return;
                }

                var data = new BrownianDataModel
                {
                    InitialPrice = s0,
                    Sigma = vol / 100.0,
                    Mean = mu / 100.0,
                    NumDays = (int)NumDays
                };

                var allSimulationsData = new List<double[]>();
                var tasks = new List<Task<double[]>>();

                // Gera todas as simulações em paralelo de forma segura
                for (int i = 0; i < (int)NumberOfSimulations; i++)
                {
                    int seed = _random.Next();
                    tasks.Add(Task.Run(() => _brownianService.GenerateSimulation(data, new Random(seed))));
                }

                var results = await Task.WhenAll(tasks);

                _mainThreadInvoker.BeginInvokeOnMainThread(() =>
                {
                    LastSimulationData = results.ToList();
                });

            }
            catch (Exception ex)
            {
                await _dialogService.DisplayAlert("Erro na Simulação", $"Ocorreu um erro inesperado: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }



        [RelayCommand(CanExecute = nameof(CanClearChart))]
        private void ClearSimulation()
        {
            LastSimulationData = null;
        }

        public bool CanClearChart => LastSimulationData != null && LastSimulationData.Any();

        private bool TryParseInputs(out double s0, out double vol, out double mu)
        {
            bool s0_ok = double.TryParse(InitialPrice, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out s0);
            bool vol_ok = double.TryParse(Sigma, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out vol);
            bool mu_ok = double.TryParse(Mean, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out mu);
            return s0_ok && vol_ok && mu_ok;
        }
    }
}
