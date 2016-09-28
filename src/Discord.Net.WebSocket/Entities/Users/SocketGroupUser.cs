using Discord.Rest;
using System.Diagnostics;
using Model = Discord.API.User;

namespace Discord.WebSocket
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class SocketGroupUser : SocketUser, IGroupUser
    {
        internal SocketGroupUser(DiscordSocketClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal new static SocketGroupUser Create(DiscordSocketClient discord, Model model)
        {
            var entity = new SocketGroupUser(discord, model.Id);
            entity.Update(model);
            return entity;
        }

        //IVoiceState
        bool IVoiceState.IsDeafened => false;
        bool IVoiceState.IsMuted => false;
        bool IVoiceState.IsSelfDeafened => false;
        bool IVoiceState.IsSelfMuted => false;
        bool IVoiceState.IsSuppressed => false;
        IVoiceChannel IVoiceState.VoiceChannel => null;
        string IVoiceState.VoiceSessionId => null;
    }
}
