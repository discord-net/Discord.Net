using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface IStateProvider
    {
        ValueTask<IPresence> GetPresenceAsync(ulong userId, StateBehavior stateBehavior);
        ValueTask AddOrUpdatePresenseAsync(ulong userId, IPresence presense, StateBehavior stateBehavior);
        ValueTask RemovePresenseAsync(ulong userId);

        ValueTask<IUser> GetUserAsync(ulong id, StateBehavior stateBehavior, RequestOptions options = null);
        ValueTask<IEnumerable<IUser>> GetUsersAsync(StateBehavior stateBehavior, RequestOptions options = null);
        ValueTask AddOrUpdateUserAsync(IUser user);
        ValueTask RemoveUserAsync(ulong id);

        ValueTask<IGuildUser> GetMemberAsync(ulong guildId, ulong id, StateBehavior stateBehavior, RequestOptions options = null);
        ValueTask<IEnumerable<IGuildUser>> GetMembersAsync(ulong guildId, StateBehavior stateBehavior, RequestOptions options = null);
        ValueTask AddOrUpdateMemberAsync(ulong guildId, IGuildUser user);
        ValueTask RemoveMemberAsync(ulong guildId, ulong id);
    }
}
