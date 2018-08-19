using System.Collections.Generic;
using System.Linq;
using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class ChannelCreateAuditLogData : IAuditLogData
    {
        private ChannelCreateAuditLogData(ulong id, string name, ChannelType type,
            IReadOnlyCollection<Overwrite> overwrites)
        {
            ChannelId = id;
            ChannelName = name;
            ChannelType = type;
            Overwrites = overwrites;
        }

        public ulong ChannelId { get; }
        public string ChannelName { get; }
        public ChannelType ChannelType { get; }
        public IReadOnlyCollection<Overwrite> Overwrites { get; }

        internal static ChannelCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var overwritesModel = changes.FirstOrDefault(x => x.ChangedProperty == "permission_overwrites");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");

            var type = typeModel.NewValue.ToObject<ChannelType>();
            var name = nameModel.NewValue.ToObject<string>();

            var overwrites = (from overwrite in overwritesModel.NewValue
                let deny = overwrite.Value<ulong>("deny")
                let _type = overwrite.Value<string>("type")
                let id = overwrite.Value<ulong>("id")
                let allow = overwrite.Value<ulong>("allow")
                let permType = _type == "member" ? PermissionTarget.User : PermissionTarget.Role
                select new Overwrite(id, permType, new OverwritePermissions(allow, deny))).ToList();

            return new ChannelCreateAuditLogData(entry.TargetId.Value, name, type, overwrites.ToReadOnlyCollection());
        }
    }
}
