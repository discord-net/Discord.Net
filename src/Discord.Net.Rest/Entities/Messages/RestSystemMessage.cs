using System.Diagnostics;
using Model = Discord.API.Message;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestSystemMessage : RestMessage, ISystemMessage
    {
        public MessageType Type { get; private set; }

        internal RestSystemMessage(BaseDiscordClient discord, ulong id, ulong channelId, RestUser author)
            : base(discord, id, channelId, author)
        {
        }
        internal new static RestSystemMessage Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestSystemMessage(discord, model.Id, model.ChannelId, RestUser.Create(discord, model.Author.Value));
            entity.Update(model);
            return entity;
        }
        internal override void Update(Model model)
        {
            base.Update(model);

            Type = model.Type;
        }

        private string DebuggerDisplay => $"{Author}: {Content} ({Id}, {Type})";
    }
}
