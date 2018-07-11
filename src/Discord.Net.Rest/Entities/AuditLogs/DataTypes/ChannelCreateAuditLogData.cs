using System.Collections.Generic;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a piece of audit log data related to a channel creation.
    /// </summary>
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

            var type = typeModel.NewValue.ToObject<ChannelType>();
            var name = nameModel.NewValue.ToObject<string>();

            foreach (var overwrite in overwritesModel.NewValue)
            {
                var deny = overwrite.Value<ulong>("deny");
                var _type = overwrite.Value<string>("type");
                var id = overwrite.Value<ulong>("id");
                var allow = overwrite.Value<ulong>("allow");

                PermissionTarget permType = _type == "member" ? PermissionTarget.User : PermissionTarget.Role;

                overwrites.Add(new Overwrite(id, permType, new OverwritePermissions(allow, deny)));
            }

            return new ChannelCreateAuditLogData(entry.TargetId.Value, name, type, overwrites.ToReadOnlyCollection());
        }

        /// <summary>
        ///     Gets the snowflake ID of the created channel.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the created channel.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the name of the created channel.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the created channel.
        /// </returns>
        public string ChannelName { get; }
        /// <summary>
        ///     Gets the type of the created channel.
        /// </summary>
        /// <returns>
        ///     The type of channel that was created.
        /// </returns>
        public ChannelType ChannelType { get; }
        /// <summary>
        ///     Gets a collection of permission overwrites that was assigned to the created channel.
        /// </summary>
        /// <returns>
        ///     A collection of permission <see cref="Overwrite"/>.
        /// </returns>
        public IReadOnlyCollection<Overwrite> Overwrites { get; }
    }
}
