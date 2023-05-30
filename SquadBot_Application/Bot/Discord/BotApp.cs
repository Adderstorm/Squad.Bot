using Discord.WebSocket;
using Discord;
using SquadBot_Application.Models;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using SquadBot_Application.Bot.Data;

namespace SquadBot_Application.Bot.Discord
{
    internal class BotApp
    {
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        private readonly Config? _config;

        private readonly DiscordSocketConfig _socketConfig = new()
        {
            GatewayIntents = GatewayIntents.All,
            // Download users so that all users are available in large guilds
            AlwaysDownloadUsers = true
        };

        public BotApp(Config? config)
        {
            _config = config;

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            var options = new DbContextOptionsBuilder<SquadDBContext>()
                .UseSqlite(config.DbOptions)
                .Options;

            // Add services to dependency injection
            _services = new ServiceCollection()
                .AddSingleton(_socketConfig)
                .AddSingleton(_configuration)
                .AddSingleton(new SquadDBContext(options))
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .BuildServiceProvider();
        }

        internal static bool IsDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        internal async Task<Exception?> RunAsync()
        {
            try
            {
                var _discordClient = _services.GetRequiredService<DiscordSocketClient>();

                await _services.GetRequiredService<InteractionHandler>()
                .InitializeAsync();

                // Login and start bot
                await _discordClient.LoginAsync(TokenType.Bot, _config.Token);
                await _discordClient.StartAsync();

                // Block the task indefinitely
                await Task.Delay(Timeout.Infinite);
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }
    }
}
