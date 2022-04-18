using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public interface ICacheProvider
    {
        #region Users

        ValueTask<IUserModel> GetUserAsync(ulong id, CacheRunMode runmode);
        ValueTask<IEnumerable<IUserModel>> GetUsersAsync(CacheRunMode runmode);
        ValueTask AddOrUpdateUserAsync(IUserModel model, CacheRunMode runmode);
        ValueTask RemoveUserAsync(ulong id, CacheRunMode runmode);

        #endregion

        #region Members

        ValueTask<IMemberModel> GetMemberAsync(ulong id, ulong guildId, CacheRunMode runmode);
        ValueTask<IEnumerable<IMemberModel>> GetMembersAsync(ulong guildId, CacheRunMode runmode);
        ValueTask AddOrUpdateMemberAsync(IMemberModel model, ulong guildId, CacheRunMode runmode);
        ValueTask RemoveMemberAsync(ulong id, ulong guildId, CacheRunMode runmode);

        #endregion

        #region Presence

        ValueTask<IPresenceModel> GetPresenceAsync(ulong userId, CacheRunMode runmode);
        ValueTask AddOrUpdatePresenseAsync(ulong userId, IPresenceModel model, CacheRunMode runmode);
        ValueTask RemovePresenseAsync(ulong userId, CacheRunMode runmode);

        #endregion
    }
}
