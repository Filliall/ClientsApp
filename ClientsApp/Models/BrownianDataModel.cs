namespace ClientsApp.Models
{
    public class BrownianDataModel
    {
        public double InitialPrice { get; set; }
        public double Sigma { get; set; } // Volatility
        public double Mean { get; set; }  // Drift
        public int NumDays { get; set; }
    }
}