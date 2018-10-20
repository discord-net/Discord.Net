using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    /// <summary>
    ///     Contains a piece of audit log data related to an invite removal.
    /// </summary>
    public class InviteDeleteAuditLogData : IAuditLogData
    {
        private InviteDeleteAuditLogData(int maxAge, string code, bool temporary, IUser inviter, ulong channelId, int uses, int maxUses)
        {
            MaxAge = maxAge;
            Code = code;
            Temporary = temporary;
            Creator = inviter;
            ChannelId = channelId;
            Uses = uses;
            MaxUses = maxUses;
        }

        internal static InviteDeleteAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var maxAgeModel = changes.FirstOrDefault(x => x.ChangedProperty == "max_age");
            var codeModel = changes.FirstOrDefault(x => x.ChangedProperty == "code");
            var temporaryModel = changes.FirstOrDefault(x => x.ChangedProperty == "temporary");
            var inviterIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "inviter_id");
            var channelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var usesModel = changes.FirstOrDefault(x => x.ChangedProperty == "uses");
            var maxUsesModel = changes.FirstOrDefault(x => x.ChangedProperty == "max_uses");

            var maxAge = maxAgeModel.OldValue.ToObject<int>(discord.ApiClient.Serializer);
            var code = codeModel.OldValue.ToObject<string>(discord.ApiClient.Serializer);
            var temporary = temporaryModel.OldValue.ToObject<bool>(discord.ApiClient.Serializer);
            var inviterId = inviterIdModel.OldValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var channelId = channelIdModel.OldValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var uses = usesModel.OldValue.ToObject<int>(discord.ApiClient.Serializer);
            var maxUses = maxUsesModel.OldValue.ToObject<int>(discord.ApiClient.Serializer);

            var inviterInfo = log.Users.FirstOrDefault(x => x.Id == inviterId);
            var inviter = RestUser.Create(discord, inviterInfo);

            return new InviteDeleteAuditLogData(maxAge, code, temporary, inviter, channelId, uses, maxUses);
        }

        /// <summary>
        ///     Gets the time (in seconds) until the invite expires.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the time in seconds until this invite expires.
        /// </returns>
        public int MaxAge { get; }
        /// <summary>
        ///     Gets the unique identifier for this invite.
        /// </summary>
        /// <returns>
        ///     A string containing the invite code (e.g. <c>FTqNnyS</c>).
        /// </returns>
        public string Code { get; }
        /// <summary>
        ///     Gets a value that indicates whether the invite is a temporary one.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if users accepting this invite will be removed from the guild when they log off; otherwise
        ///     <c>false</c>.
        /// </returns>
        public bool Temporary { get; }
        /// <summary>
        ///     Gets the user that created this invite.
        /// </summary>
        /// <returns>
        ///     A user that created this invite.
        /// </returns>
        public IUser Creator { get; }
        /// <summary>
        ///     Gets the ID of the channel this invite is linked to.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the channel snowflake identifier that the invite points to.
        /// </returns>
        public ulong ChannelId { get; }
        /// <summary>
        ///     Gets the number of times this invite has been used.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the number of times this invite has been used.
        /// </returns>
        public int Uses { get; }
        /// <summary>
        ///     Gets the max number of uses this invite may have.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the number of uses this invite may be accepted until it is removed
        ///     from the guild; <c>null</c> if none is set.
        /// </returns>
        public int MaxUses { get; }
    }
}
