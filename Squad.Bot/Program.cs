using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Squad.Bot.Data;
using Squad.Bot.Discord;
using Squad.Bot.Logging;

namespace Squad.Bot
{
    internal class Program
    {
        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        private DiscordSocketClient _client = null!;

        static void Main() => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            using IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices((_, services) =>
                services.AddSingleton(_configuration)
                .AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.All,
                    LogGatewayIntentWarnings = false,
                    AlwaysDownloadUsers = true,
                    LogLevel = LogSeverity.Debug,
                    TotalShards = Convert.ToInt16(_configuration["BotSettings:totalShards"]),
                }))
                .AddDbContext<SquadDBContext>(options => options.UseSqlite(_configuration["ConnectionStrings:DbConnection"]))
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>())
                .Build();
            await RunAsync(host);
        }

        private async Task RunAsync(IHost host)
        {
            using IServiceScope serviceScope = host.Services.CreateAsyncScope();
            IServiceProvider services = serviceScope.ServiceProvider;

            var commands = services.GetRequiredService<InteractionService>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            var config = services.GetRequiredService<IConfiguration>();

            // Here we can initialize the service that will register and execute our commands
            await services.GetRequiredService<InteractionHandler>().InitializeAsync();

            // Bot token can be provided from the Configuration object we set up earlier
            await _client.LoginAsync(TokenType.Bot, config["BotSettings:token"]);
            await _client.StartAsync();

            await Logger.LogInfo("Bot has been started");

            // Block the task indefinitely
            await Task.Delay(Timeout.Infinite);
        }
    }
}