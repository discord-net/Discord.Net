using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class EmoteUpdateChanges : IAuditLogChanges
    {
        private EmoteUpdateChanges(Emote emote, string oldName)
        {
            Emote = emote;
            PreviousName = oldName;
        }

        internal static EmoteUpdateChanges Create(BaseDiscordClient discord, EntryModel entry, ChangeModel model)
        {
            var emoteName = model.NewValue.ToObject<string>();
            var emote = new Emote(entry.TargetId.Value, emoteName);

            var oldName = model.OldValue.ToObject<string>();

            return new EmoteUpdateChanges(emote, oldName);
        }

        public Emote Emote { get; }
        public string PreviousName { get; }
    }
}
