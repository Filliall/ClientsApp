namespace ClientsApp.Services
{
    public class AppNavigationService : IAppNavigationService
    {
        public Task GoToAsync(string route, bool animate = true)
        {
            return Shell.Current.GoToAsync(route, animate);
        }

        public Task GoToAsync(string route, bool animate, IDictionary<string, object> parameters)
        {
            return Shell.Current.GoToAsync(route, animate, parameters);
        }

        public Task GoBackAsync()
        {
            return Shell.Current.GoToAsync("..");
        }
    }
}