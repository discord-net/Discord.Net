using System.Linq;
using EntryModel = Discord.API.AuditLogEntry;
using Model = Discord.API.AuditLog;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a thread update.
    /// </summary>
    public class ThreadUpdateAuditLogData : IAuditLogData
    {
        private ThreadUpdateAuditLogData(IThreadChannel thread, ThreadType type, ThreadInfo before, ThreadInfo after)
        {
            Thread = thread;
            ThreadType = type;
            Before = before;
            After = after;
        }

        internal static ThreadUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var id = entry.TargetId.Value;

            var nameModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var typeModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "type");

            var archivedModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "archived");
            var autoArchiveDurationModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "auto_archive_duration");
            var lockedModel = entry.Changes.FirstOrDefault(x => x.ChangedProperty == "locked");
            var rateLimitPerUserModel = changes.FirstOrDefault(x => x.ChangedProperty == "rate_limit_per_user");

            var type = typeModel.OldValue.ToObject<ThreadType>(discord.ApiClient.Serializer);

            var oldName = nameModel.OldValue.ToObject<string>(discord.ApiClient.Serializer);
            var oldArchived = archivedModel.OldValue.ToObject<bool>(discord.ApiClient.Serializer);
            var oldAutoArchiveDuration = autoArchiveDurationModel.OldValue.ToObject<ThreadArchiveDuration>(discord.ApiClient.Serializer);
            var oldLocked = lockedModel.OldValue.ToObject<bool>(discord.ApiClient.Serializer);
            var oldRateLimit = rateLimitPerUserModel?.OldValue?.ToObject<int>(discord.ApiClient.Serializer);
            var before = new ThreadInfo(oldName, oldArchived, oldAutoArchiveDuration, oldLocked, oldRateLimit);

            var newName = nameModel.NewValue.ToObject<string>(discord.ApiClient.Serializer);
            var newArchived = archivedModel.NewValue.ToObject<bool>(discord.ApiClient.Serializer);
            var newAutoArchiveDuration = autoArchiveDurationModel.NewValue.ToObject<ThreadArchiveDuration>(discord.ApiClient.Serializer);
            var newLocked = lockedModel.NewValue.ToObject<bool>(discord.ApiClient.Serializer);
            var newRateLimit = rateLimitPerUserModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);
            var after = new ThreadInfo(newName, newArchived, newAutoArchiveDuration, newLocked, newRateLimit);

            var threadInfo = log.Threads.FirstOrDefault(x => x.Id == id);
            var threadChannel = threadInfo == null ? null : RestThreadChannel.Create(discord, (IGuild)null, threadInfo);

            return new ThreadUpdateAuditLogData(threadChannel, type, before, after);
        }

        // Doc Note: Corresponds to the *current* data

        /// <summary>
        ///     Gets the thread that was created if it still exists.
        /// </summary>
        /// <returns>
        ///     A thread object representing the thread that was created if it still exists, otherwise returns <c>null</c>.
        /// </returns>
        public IThreadChannel Thread { get; }
        /// <summary>
        ///     Gets the type of the thread.
        /// </summary>
        /// <returns>
        ///     The type of thread.
        /// </returns>
        public ThreadType ThreadType { get; }
        /// <summary>
        ///     Gets the thread information before the changes.
        /// </summary>
        /// <returns>
        ///     A thread information object representing the thread before the changes were made.
        /// </returns>
        public ThreadInfo Before { get; }
        /// <summary>
        ///     Gets the thread information after the changes.
        /// </summary>
        /// <returns>
        ///     A thread information object representing the thread after the changes were made.
        /// </returns>
        public ThreadInfo After { get; }
    }
}
