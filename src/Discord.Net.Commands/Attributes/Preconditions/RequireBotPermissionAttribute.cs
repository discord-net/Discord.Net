using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequireBotPermissionAttribute : PreconditionAttribute
    {
        public GuildPermission? GuildPermission { get; }
        public ChannelPermission? ChannelPermission { get; }

        public RequireBotPermissionAttribute(GuildPermission permission)
        {
            GuildPermission = permission;
            ChannelPermission = null;
        }
        public RequireBotPermissionAttribute(ChannelPermission permission)
        {
            ChannelPermission = permission;
            GuildPermission = null;
        }

        public override async Task<PreconditionResult> CheckPermissions(CommandContext context, CommandInfo command, IDependencyMap map)
        {
            var guildUser = await context.Guild.GetCurrentUserAsync();

            if (GuildPermission.HasValue)
            {
                if (guildUser == null)
                    return PreconditionResult.FromError("Command must be used in a guild channel");
                if (!guildUser.GuildPermissions.Has(GuildPermission.Value))
                    return PreconditionResult.FromError($"Command requires guild permission {GuildPermission.Value}");
            }

            if (ChannelPermission.HasValue)
            {
                var guildChannel = context.Channel as IGuildChannel;

                ChannelPermissions perms;
                if (guildChannel != null)
                    perms = guildUser.GetPermissions(guildChannel);
                else
                    perms = ChannelPermissions.All(guildChannel);

                if (!perms.Has(ChannelPermission.Value))
                    return PreconditionResult.FromError($"Command requires channel permission {ChannelPermission.Value}");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
