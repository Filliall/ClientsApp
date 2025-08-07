namespace ClientsApp.Services
{
    public class RealMainThreadInvoker : IMainThreadInvoker
    {
        public void BeginInvokeOnMainThread(Action action)
        {
            MainThread.BeginInvokeOnMainThread(action);
        }
    }
}