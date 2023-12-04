namespace Squad.Bot
{
    internal class Program
    {
        static void Main() => new Startup().Initialize().GetAwaiter().GetResult();

    }
}