using System;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    internal class RoleTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(IMessage context, string input)
        {
            IGuildChannel guildChannel = context.Channel as IGuildChannel;

            if (guildChannel != null)
            {
                //By Id
                ulong id;
                if (MentionUtils.TryParseRole(input, out id) || ulong.TryParse(input, out id))
                {
                    var channel = guildChannel.Guild.GetRole(id);
                    if (channel != null)
                        return Task.FromResult(TypeReaderResult.FromSuccess(channel));
                }

                //By Name
                var roles = guildChannel.Guild.Roles;
                var filteredRoles = roles.Where(x => string.Equals(input, x.Name, StringComparison.OrdinalIgnoreCase)).ToArray();
                if (filteredRoles.Length > 1)
                    return Task.FromResult(TypeReaderResult.FromError(CommandError.MultipleMatches, "Multiple roles found."));
                else if (filteredRoles.Length == 1)
                    return Task.FromResult(TypeReaderResult.FromSuccess(filteredRoles[0]));
            }
            
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, "Role not found."));
        }
    }
}
