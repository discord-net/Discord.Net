using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal class SocketInviteHelper
    {
        public static async Task DeleteAsync(ISocketInvite invite, BaseSocketClient client,
           RequestOptions options)
        {
            await client.ApiClient.DeleteInviteAsync(invite.Code, options).ConfigureAwait(false);
        }
    }
}
