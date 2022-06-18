using System;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Requires the user invoking the command to have a specified permission.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireUserPermissionAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     Gets the specified <see cref="Discord.GuildPermission" /> of the precondition.
        /// </summary>
        public GuildPermission? GuildPermission { get; }
        /// <summary>
        ///     Gets the specified <see cref="Discord.ChannelPermission" /> of the precondition.
        /// </summary>
        public ChannelPermission? ChannelPermission { get; }
        /// <summary>
        ///     Gets or sets the error message if the precondition
        ///     fails due to being run outside of a Guild channel.
        /// </summary>
        public string NotAGuildErrorMessage { get; set; }

        /// <summary>
        ///     Requires that the user invoking the command to have a specific <see cref="Discord.GuildPermission"/>.
        /// </summary>
        /// <remarks>
        ///     This precondition will always fail if the command is being invoked in a <see cref="IPrivateChannel"/>.
        /// </remarks>
        /// <param name="guildPermission">
        ///     The <see cref="Discord.GuildPermission" /> that the user must have. Multiple permissions can be
        ///     specified by ORing the permissions together.
        /// </param>
        public RequireUserPermissionAttribute(GuildPermission guildPermission)
        {
            GuildPermission = guildPermission;
        }

        /// <summary>
        ///     Requires that the user invoking the command to have a specific <see cref="Discord.ChannelPermission"/>.
        /// </summary>
        /// <param name="channelPermission">
        ///     The <see cref="Discord.ChannelPermission"/> that the user must have. Multiple permissions can be
        ///     specified by ORing the permissions together.
        /// </param>
        public RequireUserPermissionAttribute(ChannelPermission channelPermission)
        {
            ChannelPermission = channelPermission;
        }

        /// <inheritdoc />
        public override Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
        {
            var guildUser = context.User as IGuildUser;

            if (GuildPermission.HasValue)
            {
                if (guildUser == null)
                    return Task.FromResult(PreconditionResult.FromError(NotAGuildErrorMessage ?? "Command must be used in a guild channel."));
                if (!guildUser.GuildPermissions.Has(GuildPermission.Value))
                    return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? $"User requires guild permission {GuildPermission.Value}."));
            }

            if (ChannelPermission.HasValue)
            {
                ChannelPermissions perms;
                if (context.Channel is IGuildChannel guildChannel)
                    perms = guildUser.GetPermissions(guildChannel);
                else
                    perms = ChannelPermissions.All(context.Channel);

                if (!perms.Has(ChannelPermission.Value))
                    return Task.FromResult(PreconditionResult.FromError(ErrorMessage ?? $"User requires channel permission {ChannelPermission.Value}."));
            }

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
