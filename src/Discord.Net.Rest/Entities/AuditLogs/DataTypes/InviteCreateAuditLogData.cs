using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;

namespace Discord.Rest
{
    public class InviteCreateAuditLogData : IAuditLogData
    {
        private InviteCreateAuditLogData(int maxAge, string code, bool temporary, IUser inviter, ulong channelId, int uses, int maxUses)
        {
            MaxAge = maxAge;
            Code = code;
            Temporary = temporary;
            Creator = inviter;
            ChannelId = channelId;
            Uses = uses;
            MaxUses = maxUses;
        }

        internal static InviteCreateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var maxAgeModel = changes.FirstOrDefault(x => x.ChangedProperty == "max_age");
            var codeModel = changes.FirstOrDefault(x => x.ChangedProperty == "code");
            var temporaryModel = changes.FirstOrDefault(x => x.ChangedProperty == "temporary");
            var inviterIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "inviter_id");
            var channelIdModel = changes.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var usesModel = changes.FirstOrDefault(x => x.ChangedProperty == "uses");
            var maxUsesModel = changes.FirstOrDefault(x => x.ChangedProperty == "max_uses");

            var maxAge = maxAgeModel.NewValue.ToObject<int>(discord.ApiClient.Serializer);
            var code = codeModel.NewValue.ToObject<string>(discord.ApiClient.Serializer);
            var temporary = temporaryModel.NewValue.ToObject<bool>(discord.ApiClient.Serializer);
            var inviterId = inviterIdModel.NewValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var channelId = channelIdModel.NewValue.ToObject<ulong>(discord.ApiClient.Serializer);
            var uses = usesModel.NewValue.ToObject<int>(discord.ApiClient.Serializer);
            var maxUses = maxUsesModel.NewValue.ToObject<int>(discord.ApiClient.Serializer);

            var inviterInfo = log.Users.FirstOrDefault(x => x.Id == inviterId);
            var inviter = RestUser.Create(discord, inviterInfo);

            return new InviteCreateAuditLogData(maxAge, code, temporary, inviter, channelId, uses, maxUses);
        }

        public int MaxAge { get; }
        public string Code { get; }
        public bool Temporary { get; }
        public IUser Creator { get; }
        public ulong ChannelId { get; }
        public int Uses { get; }
        public int MaxUses { get; }
    }
}
