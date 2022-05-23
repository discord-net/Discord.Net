using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public static class EntityCacheExtensions
    {
        public static ValueTask<IUser> GetUserAsync(this MessageInteraction<SocketUser> interaction, DiscordSocketClient client,
            CacheMode mode, RequestOptions options = null)
            => client.StateManager.UserStore.GetAsync(interaction.UserId, mode, options);
    }
}
