using System.Diagnostics;
using Model = Discord.API.User;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestGroupUser : RestUser, IGroupUser
    {
        internal RestGroupUser(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal new static RestGroupUser Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestGroupUser(discord, model.Id);
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
