namespace ClientsApp.Services
{
    public interface IMainThreadInvoker
    {
        void BeginInvokeOnMainThread(Action action);
    }
}