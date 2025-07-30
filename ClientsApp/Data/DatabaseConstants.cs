namespace ClientsApp.Data
{
    public static class DatabaseConstants
    {
        public const string DatabaseFilename = "ClientsSQLite.db3";

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    }
}