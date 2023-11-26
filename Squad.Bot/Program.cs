using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Squad.Bot.Data;
using Squad.Bot.Discord;

namespace Squad.Bot
{
    internal class Program
    {
        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

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

            var client = new BotClient();
            await client.RunAsync(host);
        }
    }
}