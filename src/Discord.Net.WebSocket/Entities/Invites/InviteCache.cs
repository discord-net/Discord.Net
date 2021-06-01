using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal class InviteCache
    {
        private readonly ConcurrentDictionary<string, SocketGuildInvite> _invites;
        private readonly ConcurrentQueue<string> _queue;
        private static int _size;

        public InviteCache(DiscordSocketClient client)
        {
            //NOTE:
            //This should be an option in the client config. default for now is 20 invites per guild
            _size = client.Guilds.Count * 20;

            _invites = new ConcurrentDictionary<string, SocketGuildInvite>();
            _queue = new ConcurrentQueue<string>();
        }
        public void Add(SocketGuildInvite invite)
        {
            if(_invites.TryAdd(invite.Code, invite))
            {
                _queue.Enqueue(invite.Code);

                while (_queue.Count > _size && _queue.TryDequeue(out string invCode))
                    _invites.TryRemove(invCode, out _);
            }
        }
        public SocketGuildInvite Remove(string inviteCode)
        {
            _invites.TryRemove(inviteCode, out SocketGuildInvite inv);
            return inv;
        }
        public SocketGuildInvite Get(string inviteCode)
        {
            if(_invites.TryGetValue(inviteCode, out SocketGuildInvite inv))
                return inv;
            return null;
        }
    }
}
