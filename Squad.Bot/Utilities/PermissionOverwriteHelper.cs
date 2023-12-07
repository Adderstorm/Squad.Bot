using Discord;

namespace Squad.Bot.Utilities
{
    /// <summary>
    /// A helper class for working with Discord permission overwrites.
    /// </summary>
    public class PermissionOverwriteHelper
    {
        /// <summary>
        /// Gets the unique identifier for the object this overwrite is targeting.
        /// </summary>
        public ulong TargetId { get; }

        /// <summary>
        /// Gets the type of object this overwrite is targeting.
        /// </summary>
        public PermissionTarget TargetType { get; }

        /// <summary>
        /// Gets the permissions associated with this overwrite entry.
        /// </summary>
        public OverwritePermissions Permissions { private get; set; }

        /// <summary>
        /// Initializes a new Discord.Overwrite with provided target information and modified permissions.
        /// </summary>
        /// <param name="targetId">The unique identifier for the object this overwrite is targeting.</param>
        /// <param name="targetType">The type of object this overwrite is targeting.</param>
        public PermissionOverwriteHelper(ulong targetId, PermissionTarget targetType)
        {
            TargetId = targetId;
            TargetType = targetType;
        }

        /// <summary>
        /// Gets the permission overwrites for the specified target.
        /// </summary>
        /// <returns>A collection of permission overwrites for the specified target.</returns>
        public Optional<IEnumerable<Overwrite>> CreateOverwrites()
        {
            return Optional.Create(new List<Overwrite> { new (targetId: TargetId, targetType: TargetType, permissions: Permissions) }.ToAsyncEnumerable().ToEnumerable());
        }

        /// <summary>
        /// Creates a new <see cref="OverwritePermissions"/> object with the specified permission values.
        /// </summary>
        /// <returns>A new <see cref="OverwritePermissions"/> object with the specified permission values.</returns>
        public static OverwritePermissions SetOverwritePermissions( PermValue createInstantInvite = PermValue.Inherit, 
                                                                    PermValue manageChannel = PermValue.Inherit, PermValue addReactions = PermValue.Inherit,
                                                                    PermValue viewChannel = PermValue.Inherit, PermValue sendMessages = PermValue.Inherit,
                                                                    PermValue sendTTSMessages = PermValue.Inherit, PermValue manageMessages = PermValue.Inherit,
                                                                    PermValue embedLinks = PermValue.Inherit, PermValue attachFiles = PermValue.Inherit,
                                                                    PermValue readMessageHistory = PermValue.Inherit, PermValue mentionEveryone = PermValue.Inherit,
                                                                    PermValue useExternalEmojis = PermValue.Inherit, PermValue connect = PermValue.Inherit,
                                                                    PermValue speak = PermValue.Inherit, PermValue muteMembers = PermValue.Inherit,
                                                                    PermValue deafenMembers = PermValue.Inherit, PermValue moveMembers = PermValue.Inherit,
                                                                    PermValue useVoiceActivation = PermValue.Inherit, PermValue manageRoles = PermValue.Inherit,
                                                                    PermValue manageWebhooks = PermValue.Inherit, PermValue prioritySpeaker = PermValue.Inherit,
                                                                    PermValue stream = PermValue.Inherit, PermValue useSlashCommands = PermValue.Inherit,
                                                                    PermValue useApplicationCommands = PermValue.Inherit, PermValue requestToSpeak = PermValue.Inherit,
                                                                    PermValue manageThreads = PermValue.Inherit, PermValue createPublicThreads = PermValue.Inherit,
                                                                    PermValue createPrivateThreads = PermValue.Inherit, PermValue usePublicThreads = PermValue.Inherit,
                                                                    PermValue usePrivateThreads = PermValue.Inherit, PermValue useExternalStickers = PermValue.Inherit,
                                                                    PermValue sendMessagesInThreads = PermValue.Inherit, PermValue startEmbeddedActivities = PermValue.Inherit)
        {
            return new OverwritePermissions(createInstantInvite: createInstantInvite, manageChannel: manageChannel, addReactions: addReactions, viewChannel: viewChannel, sendMessages: sendMessages, sendTTSMessages: sendTTSMessages,
                                            manageMessages: manageMessages, embedLinks: embedLinks, attachFiles: attachFiles, readMessageHistory: readMessageHistory, mentionEveryone: mentionEveryone, useExternalEmojis: useExternalEmojis,
                                            connect: connect, speak: speak, muteMembers: muteMembers, deafenMembers: deafenMembers, moveMembers: moveMembers, useVoiceActivation: useVoiceActivation, manageRoles: manageRoles, manageWebhooks: manageWebhooks,
                                            prioritySpeaker: prioritySpeaker, stream: stream, useSlashCommands: useSlashCommands, useApplicationCommands: useApplicationCommands, requestToSpeak: requestToSpeak, manageThreads: manageThreads, createPublicThreads: createPublicThreads,
                                            createPrivateThreads: createPrivateThreads, usePublicThreads: usePublicThreads, usePrivateThreads: usePrivateThreads, useExternalStickers: useExternalStickers, sendMessagesInThreads: sendMessagesInThreads, startEmbeddedActivities: startEmbeddedActivities);
        }
    }
}