using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class EmoteDeleteChanges : IAuditLogChanges
    {
        private EmoteDeleteChanges(Emote emote)
        {
            Emote = emote;
        }

        internal static EmoteDeleteChanges Create(BaseDiscordClient discord, EntryModel entry, ChangeModel model)
        {
            var emoteName = model.OldValue.ToObject<string>();
            var emote = new Emote(entry.TargetId.Value, emoteName);
           
            return new EmoteDeleteChanges(emote);
        }

        public Emote Emote { get; }
    }
}
