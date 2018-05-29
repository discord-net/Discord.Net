using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a piece of audit log data related to an emoji update.
    /// </summary>
    public class EmoteUpdateAuditLogData : IAuditLogData
    {
        private EmoteUpdateAuditLogData(ulong id, string oldName, string newName)
        {
            EmoteId = id;
            OldName = oldName;
            NewName = newName;
        }

        internal static EmoteUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var change = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");

            var newName = change.NewValue?.ToObject<string>();
            var oldName = change.OldValue?.ToObject<string>();

            return new EmoteUpdateAuditLogData(entry.TargetId.Value, oldName, newName);
        }

        /// <summary>
        ///     Gets the snowflake ID of the updated emoji.
        /// </summary>
        /// <returns>
        ///     An <see cref="System.UInt64"/> representing the snowflake identifier of the updated emoji.
        /// </returns>
        public ulong EmoteId { get; }
        /// <summary>
        ///     Gets the new name of the updated emoji.
        /// </summary>
        /// <returns>
        ///     A string containing the new name of the updated emoji.
        /// </returns>
        public string NewName { get; }
        /// <summary>
        ///     Gets the old name of the updated emoji.
        /// </summary>
        /// <returns>
        ///     A string containing the old name of the updated emoji.
        /// </returns>
        public string OldName { get; }
    }
}
