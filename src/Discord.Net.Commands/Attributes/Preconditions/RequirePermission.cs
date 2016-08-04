using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands.Attributes.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequirePermission : RequireGuildAttribute
    {
        public GuildPermission? GuildPermission { get; set; }
        public ChannelPermission? ChannelPermission { get; set; }

        public RequirePermission(GuildPermission permission)
        {
            GuildPermission = permission;
            ChannelPermission = null;
        }

        public RequirePermission(ChannelPermission permission)
        {
            ChannelPermission = permission;
            GuildPermission = null;
        }

        public override async Task<PreconditionResult> CheckPermissions(IMessage context, Command executingCommand, object moduleInstance)
        {
            var result = await base.CheckPermissions(context, executingCommand, moduleInstance).ConfigureAwait(false);

            if (!result.IsSuccess)
                return result;

            var author = context.Author as IGuildUser;

            if (GuildPermission.HasValue)
            {
                var guildPerms = author.GuildPermissions.ToList();
                if (!guildPerms.Contains(GuildPermission.Value))
                    return PreconditionResult.FromError($"User is missing guild permission {GuildPermission.Value}");
            }

            if (ChannelPermission.HasValue)
            {
                var channel = context.Channel as IGuildChannel;
                var channelPerms = author.GetPermissions(channel).ToList();

                if (!channelPerms.Contains(ChannelPermission.Value))
                    return PreconditionResult.FromError($"User is missing channel permission {ChannelPermission.Value}");
            }

            return PreconditionResult.FromSuccess();
        }
    }
}
