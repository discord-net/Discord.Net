using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    /// Requires the bot to have a specific permission in the channel a command is invoked in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireBotPermissionAttribute : PreconditionAttribute
    {
        public GuildPermission? GuildPermission { get; }
        public ChannelPermission? ChannelPermission { get; }

        /// <summary>
        /// Requires the bot account to have a specific <see cref="GuildPermission"/>.
        /// </summary>
        /// <remarks>This precondition will always fail if the command is being invoked in a private channel.</remarks>
        /// <param name="permission">The GuildPermission that the bot must have. Multiple permissions can be specified by ORing the permissions together.</param>
        public RequireBotPermissionAttribute(GuildPermission permission)
        {
            GuildPermission = permission;
            ChannelPermission = null;
        }
        /// <summary>
        /// Requires that the bot account to have a specific <see cref="ChannelPermission"/>.
        /// </summary>
        /// <param name="permission">The ChannelPermission that the bot must have. Multiple permissions can be specified by ORing the permissions together.</param>
        /// <example>
        /// <code language="c#">
        ///     [Command("permission")]
        ///     [RequireBotPermission(ChannelPermission.ManageMessages)]
        ///     public async Task Purge()
        ///     {
        ///     }
        /// </code>
        /// </example>
        public RequireBotPermissionAttribute(ChannelPermission permission)
        {
            ChannelPermission = permission;
            GuildPermission = null;
        }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildUser guildUser = null;
            if (context.Guild != null)
                guildUser = await context.Guild.GetCurrentUserAsync().ConfigureAwait(false);

            if (GuildPermission.HasValue)
            {
                if (guildUser == null)
                    return PreconditionResult.FromError("Command must be used in a guild channel");
                if (!guildUser.GuildPermissions.Has(GuildPermission.Value))
                    return PreconditionResult.FromError($"Bot requires guild permission {GuildPermission.Value}");
            }

            if (ChannelPermission.HasValue)
            {
                ChannelPermissions perms;
                if (context.Channel is IGuildChannel guildChannel)
                    perms = guildUser.GetPermissions(guildChannel);
                else
                    perms = ChannelPermissions.All(context.Channel);

                if (!perms.Has(ChannelPermission.Value))
                    return PreconditionResult.FromError($"Bot requires channel permission {ChannelPermission.Value}");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
