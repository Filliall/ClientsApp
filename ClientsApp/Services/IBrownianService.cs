using ClientsApp.Models;

namespace ClientsApp.Services
{
    public interface IBrownianService
    {
        double[] GenerateSimulation(BrownianDataModel data, Random random);
    }
}