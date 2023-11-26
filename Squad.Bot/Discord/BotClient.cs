using Discord.Interactions;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Squad.Bot.Logging;

namespace Squad.Bot.Discord
{
    public class BotClient
    {
        private DiscordSocketClient _client = null!;


        public async Task RunAsync(IHost host)
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
