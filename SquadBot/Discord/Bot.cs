using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using SquadBot_Application.Models;
using System;
using Discord.Interactions;
using Microsoft.Extensions.Configuration;
using SquadBot.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace SquadBot.Discord
{
    internal class Bot
    {
        private readonly string? _discordBotToken;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;
        
        private readonly DiscordSocketConfig _socketConfig = new()
        {
            GatewayIntents = GatewayIntents.All,
            // Download users so that all users are available in large guilds
            AlwaysDownloadUsers = true
        };

        public Bot(string? discordBotToken, Config config)
        {
            _discordBotToken = discordBotToken;

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
                .AddSingleton<SquadDBContext>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .AddSingleton(config)
                .BuildServiceProvider();

            SquadDBContext dBContext = new(options);
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
                await _discordClient.LoginAsync(TokenType.Bot, _discordBotToken);
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
