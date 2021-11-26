using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to an emoji deletion.
    /// </summary>
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

        /// <summary>
        ///     Gets the snowflake ID of the deleted emoji.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.UInt64"/> representing the snowflake identifier for the deleted emoji.
        /// </returns>
        public ulong EmoteId { get; }
        /// <summary>
        ///     Gets the name of the deleted emoji.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the deleted emoji.
        /// </returns>
        public string Name { get; }
    }
}
