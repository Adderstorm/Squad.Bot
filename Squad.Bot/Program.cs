namespace Squad.Bot
{
    internal static class Program
    {
        static void Main() => new Startup().InitializeAsync().GetAwaiter().GetResult();

    }
}