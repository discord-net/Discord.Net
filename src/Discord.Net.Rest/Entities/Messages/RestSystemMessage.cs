using System.Diagnostics;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestSystemMessage : RestMessage, ISystemMessage
    {
        public MessageType Type { get; private set; }

        internal RestSystemMessage(DiscordClient discord, ulong id, ulong channelId)
            : base(discord, id, channelId)
        {
        }
        internal new static RestSystemMessage Create(DiscordClient discord, Model model)
        {
            var entity = new RestSystemMessage(discord, model.Id, model.ChannelId);
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            Type = model.Type;
        }
    }
}
