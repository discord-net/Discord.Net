using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    ///     This attribute requires that the bot has a specified permission in the channel a command is invoked in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequireBotPermissionAttribute : PreconditionAttribute
    {
        /// <summary>
        ///     Require that the bot account has a specified GuildPermission
        /// </summary>
        /// <remarks>This precondition will always fail if the command is being invoked in a private channel.</remarks>
        /// <param name="permission">
        ///     The GuildPermission that the bot must have. Multiple permissions can be specified by ORing the
        ///     permissions together.
        /// </param>
        public RequireBotPermissionAttribute(GuildPermission permission)
        {
            GuildPermission = permission;
            ChannelPermission = null;
        }

        /// <summary>
        ///     Require that the bot account has a specified ChannelPermission.
        /// </summary>
        /// <param name="permission">
        ///     The ChannelPermission that the bot must have. Multiple permissions can be specified by ORing
        ///     the permissions together.
        /// </param>
        /// <example>
        ///     <code language="c#">
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

        public GuildPermission? GuildPermission { get; }
        public ChannelPermission? ChannelPermission { get; }

        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            CommandInfo command, IServiceProvider services)
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

            if (!ChannelPermission.HasValue) return PreconditionResult.FromSuccess();
            ChannelPermissions perms;
            if (context.Channel is IGuildChannel guildChannel)
                perms = guildUser.GetPermissions(guildChannel);
            else
                perms = ChannelPermissions.All(context.Channel);

            return !perms.Has(ChannelPermission.Value)
                ? PreconditionResult.FromError($"Bot requires channel permission {ChannelPermission.Value}")
                : PreconditionResult.FromSuccess();
        }
    }
}
