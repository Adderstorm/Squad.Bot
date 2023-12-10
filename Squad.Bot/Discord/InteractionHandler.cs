using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squad.Bot.Data;
using Squad.Bot.Events;
using Squad.Bot.Logging;
using System.Reflection;
using IResult = Discord.Interactions.IResult;

namespace Squad.Bot.Discord
{
    /// <summary>
    /// This class is responsible for handling interactions with the Discord server.
    /// It uses the InteractionService to register modules that contain commands,
    /// and the DiscordSocketClient to listen for interactions.
    /// </summary>
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructs a new instance of the InteractionHandler class.
        /// </summary>
        /// <param name="client">The DiscordSocketClient used to listen for interactions.</param>
        /// <param name="handler">The InteractionService used to register modules.</param>
        /// <param name="services">The service provider used to resolve dependencies.</param>
        /// <param name="configuration">The configuration used to read bot settings.</param>
        public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services, IConfiguration configuration)
        {
            _client = client;
            _handler = handler;
            _services = services;
            _configuration = configuration;
        }

        /// <summary>
        /// Initializes the InteractionHandler by registering modules and subscribing to events.
        /// </summary>
        public async Task InitializeAsync()
        {
            // Initialize the handler and services for communication with the server

            UserGuildEvent userGuildEvent = new (_services.GetRequiredService<SquadDBContext>());
            UserMessages userMessages = new (_services.GetRequiredService<SquadDBContext>());
            OnUserStateChange userStateChange = new (_services.GetRequiredService<SquadDBContext>());

            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += HandleInteraction;
            _client.Ready += ReadyAsync;

            // Subscribes for events
            _client.MessageReceived += userMessages.OnUserMessageReceived;
            _client.UserVoiceStateUpdated += userStateChange.OnUserVoiceStateUpdate;
            _client.UserLeft += userGuildEvent.OnUserLeftGuild;
            _client.UserJoined += userGuildEvent.OnUserJoinGuild;

            // Process the command execution results 
            _handler.InteractionExecuted += InteractionExecuted;
        }

        private async Task ReadyAsync()
        {
            // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
            // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
#if DEBUG
            await _handler.RegisterCommandsToGuildAsync(Convert.ToUInt64(_configuration["BotSettings:testGuild"]), true);
#else            
            await _handler.RegisterCommandsGloballyAsync(true);
#endif
        }

        private Task InteractionExecuted(ICommandInfo arg1, IInteractionContext arg2, IResult arg3)
        {
            return Task.CompletedTask;
        }

        private async Task HandleInteraction(SocketInteraction arg)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, arg);
                await _handler.ExecuteCommandAsync(ctx, _services);
            }
            catch (Exception? ex)
            {
                await Logger.LogException(ex, ex.Message);
#pragma warning disable CS8604 // Possible null reference argument.
                Console.WriteLine(ex.StackTrace, ex.Source);
#pragma warning restore CS8604 // Possible null reference argument.

                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}