using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class EmoteCreateChanges : IAuditLogChanges
    {
        private EmoteCreateChanges(Emote emote)
        {
            Emote = emote;
        }

        internal static EmoteCreateChanges Create(BaseDiscordClient discord, EntryModel entry, ChangeModel model)
        {
            var emoteName = model.NewValue.ToObject<string>();
            var emote = new Emote(entry.TargetId.Value, emoteName);

            return new EmoteCreateChanges(emote);
        }

        public Emote Emote { get; }
    }
}
