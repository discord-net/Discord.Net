using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class ChannelCreateAuditLogData : IAuditLogData
    {
        private ChannelCreateAuditLogData(ulong id, string name, ChannelType type, IReadOnlyCollection<Overwrite> overwrites)
        {
            ChannelId = id;
            ChannelName = name;
            ChannelType = type;
            Overwrites = overwrites;
        }

        internal static ChannelCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;
            var overwrites = new List<Overwrite>();

            var overwritesModel = changes.FirstOrDefault(x => x.ChangedProperty == "permission_overwrites");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");

            var type = typeModel.NewValue.ToObject<ChannelType>(discord.ApiClient.Serializer);
            var name = nameModel.NewValue.ToObject<string>(discord.ApiClient.Serializer);

            foreach (var overwrite in overwritesModel.NewValue)
            {
                var deny = overwrite.Value<ulong>("deny");
                var permType = overwrite.Value<PermissionTarget>("type");
                var id = overwrite.Value<ulong>("id");
                var allow = overwrite.Value<ulong>("allow");

                overwrites.Add(new Overwrite(id, permType, new OverwritePermissions(allow, deny)));
            }

            return new ChannelCreateAuditLogData(entry.TargetId.Value, name, type, overwrites.ToReadOnlyCollection());
        }

        public ulong ChannelId { get; }
        public string ChannelName { get; }
        public ChannelType ChannelType { get; }
        public IReadOnlyCollection<Overwrite> Overwrites { get; }
    }
}
