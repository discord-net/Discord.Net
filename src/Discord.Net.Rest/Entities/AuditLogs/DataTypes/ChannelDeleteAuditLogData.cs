using System.Collections.Generic;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a channel deletion.
    /// </summary>
    public class ChannelDeleteAuditLogData : IAuditLogData
    {
        private ChannelDeleteAuditLogData(ulong id, string name, ChannelType type, IReadOnlyCollection<Overwrite> overwrites)
        {
            ChannelId = id;
            ChannelName = name;
            ChannelType = type;
            Overwrites = overwrites;
        }

        internal static ChannelDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var overwritesModel = changes.FirstOrDefault(x => x.ChangedProperty == "permission_overwrites");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");

            var overwrites = overwritesModel.OldValue.ToObject<API.Overwrite[]>(discord.ApiClient.Serializer)
                .Select(x => new Overwrite(x.TargetId, x.TargetType, new OverwritePermissions(x.Allow, x.Deny)))
                .ToList();
            var type = typeModel.OldValue.ToObject<ChannelType>(discord.ApiClient.Serializer);
            var name = nameModel.OldValue.ToObject<string>(discord.ApiClient.Serializer);
            var id = entry.TargetId.Value;

            return new ChannelDeleteAuditLogData(id, name, type, overwrites.ToReadOnlyCollection());
        }

        /// <summary>
        ///     Gets the snowflake ID of the deleted channel.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier for the deleted channel.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the name of the deleted channel.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the deleted channel.
        /// </returns>
        public string ChannelName { get; }
        /// <summary>
        ///     Gets the type of the deleted channel.
        /// </summary>
        /// <returns>
        ///     The type of channel that was deleted.
        /// </returns>
        public ChannelType ChannelType { get; }
        /// <summary>
        ///     Gets a collection of permission overwrites that was assigned to the deleted channel.
        /// </summary>
        /// <returns>
        ///     A collection of permission <see cref="Overwrite"/>.
        /// </returns>
        public IReadOnlyCollection<Overwrite> Overwrites { get; }
    }
}
