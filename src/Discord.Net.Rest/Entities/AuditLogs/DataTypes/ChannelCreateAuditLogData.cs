using System.Collections.Generic;
using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to a channel creation.
    /// </summary>
    public class ChannelCreateAuditLogData : IAuditLogData
    {
        private ChannelCreateAuditLogData(ulong id, string name, ChannelType type, int? rateLimit, bool? nsfw, int? bitrate, IReadOnlyCollection<Overwrite> overwrites)
        {
            ChannelId = id;
            ChannelName = name;
            ChannelType = type;
            SlowModeInterval = rateLimit;
            IsNsfw = nsfw;
            Bitrate = bitrate;
            Overwrites = overwrites;
        }

        internal static ChannelCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;
            var overwrites = new List<Overwrite>();

            var overwritesModel = changes.FirstOrDefault(x => x.ChangedProperty == "permission_overwrites");
            var typeModel = changes.FirstOrDefault(x => x.ChangedProperty == "type");
            var nameModel = changes.FirstOrDefault(x => x.ChangedProperty == "name");
            var rateLimitPerUserModel = changes.FirstOrDefault(x => x.ChangedProperty == "rate_limit_per_user");
            var nsfwModel = changes.FirstOrDefault(x => x.ChangedProperty == "nsfw");
            var bitrateModel = changes.FirstOrDefault(x => x.ChangedProperty == "bitrate");

            var type = typeModel.NewValue.ToObject<ChannelType>(discord.ApiClient.Serializer);
            var name = nameModel.NewValue.ToObject<string>(discord.ApiClient.Serializer);
            int? rateLimitPerUser = rateLimitPerUserModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);
            bool? nsfw = nsfwModel?.NewValue?.ToObject<bool>(discord.ApiClient.Serializer);
            int? bitrate = bitrateModel?.NewValue?.ToObject<int>(discord.ApiClient.Serializer);

            foreach (var overwrite in overwritesModel.NewValue)
            {
                var deny = overwrite["deny"].ToObject<ulong>(discord.ApiClient.Serializer);
                var permType = overwrite["type"].ToObject<PermissionTarget>(discord.ApiClient.Serializer);
                var id = overwrite["id"].ToObject<ulong>(discord.ApiClient.Serializer);
                var allow = overwrite["allow"].ToObject<ulong>(discord.ApiClient.Serializer);

                overwrites.Add(new Overwrite(id, permType, new OverwritePermissions(allow, deny)));
            }

            return new ChannelCreateAuditLogData(entry.TargetId.Value, name, type, rateLimitPerUser, nsfw, bitrate, overwrites.ToReadOnlyCollection());
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
        ///     Gets the current slow-mode delay of the created channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="Int32"/> representing the time in seconds required before the user can send another
        ///     message; <c>0</c> if disabled.
        ///     <c>null</c> if this is not mentioned in this entry.
        /// </returns>
        public int? SlowModeInterval { get; }
        /// <summary>
        ///     Gets the value that indicates whether the created channel is NSFW.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the created channel has the NSFW flag enabled; otherwise <c>false</c>.
        ///     <c>null</c> if this is not mentioned in this entry.
        /// </returns>
        public bool? IsNsfw { get; }
        /// <summary>
        ///     Gets the bit-rate that the clients in the created voice channel are requested to use.
        /// </summary>
        /// <returns>
        ///     An <see cref="Int32"/> representing the bit-rate (bps) that the created voice channel defines and requests the
        ///     client(s) to use.
        ///     <c>null</c> if this is not mentioned in this entry.
        /// </returns>
        public int? Bitrate { get; }
        /// <summary>
        ///     Gets a collection of permission overwrites that was assigned to the created channel.
        /// </summary>
        /// <returns>
        ///     A collection of permission <see cref="Overwrite"/>, containing the permission overwrites that were
        ///     assigned to the created channel.
        /// </returns>
        public IReadOnlyCollection<Overwrite> Overwrites { get; }
    }
}
