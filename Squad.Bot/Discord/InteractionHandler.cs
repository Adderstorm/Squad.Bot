using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Squad.Bot.DsEvents;
using Squad.Bot.Logging;
using System.Reflection;
using IResult = Discord.Interactions.IResult;
using LogMessage = Discord.LogMessage;

namespace Squad.Bot.Discord
{
    public class InteractionHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;
        private readonly IConfiguration _configuration;

        // Using constructor injection
        public InteractionHandler(DiscordSocketClient client, InteractionService handler, IServiceProvider services, IConfiguration configuration)
        {
            _client = client;
            _handler = handler;
            _services = services;
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _handler.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // Process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += HandleInteraction;
            _client.Log += Log;
            _client.Ready += ReadyAsync;

            //Subscribes for events
            _client.MessageReceived += UserMessages.OnUserMessageReceived;
            _client.UserVoiceStateUpdated += OnUserStateChange.OnUserVoiceStateUpdate;
            _client.UserLeft += UserGuildEvent.OnUserLeftGuild;
            _client.UserJoined += UserGuildEvent.OnUserJoinGuild;

            // Process the command execution results 
            _handler.InteractionExecuted += InteractionExecuted;
        }

        private Task Log(LogMessage message)
        {
            Console.WriteLine(message);

            return Task.CompletedTask;
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
                await Logger.LogException(ex);
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                Console.WriteLine(ex.StackTrace, ex.Source);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.

                // If a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (arg.Type == InteractionType.ApplicationCommand)
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }
}