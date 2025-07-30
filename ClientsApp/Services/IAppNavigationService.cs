namespace ClientsApp.Services
{
    public interface IAppNavigationService
    {
        Task GoToAsync(string route, bool animate = true);
        Task GoToAsync(string route, bool animate, IDictionary<string, object> parameters);
        Task GoBackAsync();
    }
}