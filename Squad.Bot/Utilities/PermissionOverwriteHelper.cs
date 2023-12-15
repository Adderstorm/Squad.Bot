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
        public Optional<IEnumerable<Overwrite>> CreateOptionalOverwrites()
        {
            return Optional.Create(new List<Overwrite> { new (targetId: TargetId, targetType: TargetType, permissions: Permissions) }.ToAsyncEnumerable().ToEnumerable());
        }
    }
}