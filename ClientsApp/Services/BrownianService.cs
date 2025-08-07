using ClientsApp.Models;

namespace ClientsApp.Services
{
    public class BrownianService : IBrownianService
    {
        public double[] GenerateSimulation(BrownianDataModel data, Random random)
        {

            double[] prices = new double[data.NumDays];
            prices[0] = data.InitialPrice;
            for (int i = 1; i < data.NumDays; i++)
            {

                double u1 = 1.0 - random.NextDouble();
                double u2 = 1.0 - random.NextDouble();
                double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

                var r = data.Mean + data.Sigma * z;

                prices[i] = prices[i - 1] * Math.Exp(r);
            }
            return prices;
        }
    }
}