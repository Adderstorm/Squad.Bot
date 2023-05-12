namespace SquadBot_Application.Constants
{
    public static class PathConstants
    {
        private static string EnvironmentConfigFolder => Path.Combine(Environment.CurrentDirectory, "Properties");
        public static string ConfigFile => Path.Combine(EnvironmentConfigFolder, "config.json");
    }
}
