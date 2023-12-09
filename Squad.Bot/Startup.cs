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
    public class Startup
    {
        /// <summary>
        /// The configuration object used to read bot settings from appsettings.json.
        /// </summary>
        private readonly IConfiguration _configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();

        private DiscordSocketClient? _client;


        /// <summary>
        /// Initializes the bot by setting up its services and dependencies.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous initialization of the bot.</returns>
        public async Task InitializeAsync()
        {
            using IHost host = HostBuild();

            await RunAsync(host);
        }

        /// <summary>
        /// Runs the bot by starting its services and listening for incoming messages.
        /// </summary>
        /// <param name="host">The host that contains the bot's services and dependencies.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous running of the bot.</returns>
        private async Task RunAsync(IHost host)
        {
            // Create db if not exists, lol
            host.CreateDbIfNotExists();

            using IServiceScope serviceScope = host.Services.CreateAsyncScope();
            IServiceProvider services = serviceScope.ServiceProvider;

            // Get the DiscordSocketClient service from the service provider
            _client = services.GetRequiredService<DiscordSocketClient>();

            // Initialize the InteractionHandler service, which will register and execute commands
            await services.GetRequiredService<InteractionHandler>().InitializeAsync();

            // Login to Discord using the bot token from appsettings.json
            await _client.LoginAsync(TokenType.Bot, _configuration["BotSettings:Token"]);

            // Start the Discord client and listen for incoming messages
            await _client.StartAsync();

            // Log a message to the console to indicate that the bot has started
            await Logger.LogInfo("Bot has been started");

            // Wait for the bot to stop running
            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// Builds the host that contains the bot's services and dependencies.
        /// </summary>
        /// <returns>The host that contains the bot's services and dependencies.</returns>
        private IHost HostBuild() => Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Add the configuration object to the service collection
                services.AddSingleton(_configuration);

                // Add the DiscordSocketClient service to the service collection, using the DiscordSocketConfig class to configure the client
                services.AddSingleton(x => new DiscordSocketClient(new DiscordSocketConfig
                {
                    // Set the GatewayIntents property to All to enable all gateway intents
                    GatewayIntents = GatewayIntents.All,
                    // Set the LogGatewayIntentWarnings property to false to disable logging of gateway intent warnings
                    LogGatewayIntentWarnings = false,
                    // Set the AlwaysDownloadUsers property to true to enable automatic downloading of users that the bot interacts with
                    AlwaysDownloadUsers = true,
                    // Set the LogLevel property to Debug to enable debug-level logging
                    LogLevel = LogSeverity.Debug,
                    // Set the TotalShards property to the number of shards specified in appsettings.json
                    TotalShards = Convert.ToInt16(_configuration["BotSettings:TotalShards"])
                }));

                // Add the SquadDBContext service to the service collection, using the provided connection string to configure the context
                services.AddDbContext<SquadDBContext>(options => options.UseSqlite(_configuration["ConnectionStrings:DbConnection"]));

                // Add the InteractionService service to the service collection, using the DiscordSocketClient service that was just added
                services.AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()));

                // Add the InteractionHandler service to the service collection, using the InteractionService that was just added
                services.AddSingleton<InteractionHandler>();
            })
            .Build();
    }
}
