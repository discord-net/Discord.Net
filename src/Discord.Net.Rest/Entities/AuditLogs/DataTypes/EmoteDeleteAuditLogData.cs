using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class EmoteDeleteAuditLogData : IAuditLogData
    {
        private EmoteDeleteAuditLogData(ulong id, string name)
        {
            EmoteId = id;
            Name = name;
        }

        internal static EmoteDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var change = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");

            var emoteName = change.OldValue?.ToObject<string>(discord.ApiClient.Serializer);

            return new EmoteDeleteAuditLogData(entry.TargetId.Value, emoteName);
        }

        public ulong EmoteId { get; }
        public string Name { get; }
    }
}
