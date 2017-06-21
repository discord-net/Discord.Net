using System.Linq;

using Model = Discord.API.AuditLog;
using ChangeModel = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class InviteCreateChanges : IAuditLogChanges
    {
        private InviteCreateChanges(int maxAge, string code, bool temporary, IUser inviter, ulong channelId, int uses, int maxUses)
        {
            MaxAge = maxAge;
            Code = code;
            Temporary = temporary;
            Creator = inviter;
            ChannelId = channelId;
            Uses = uses;
            MaxUses = maxUses;
        }

        internal static InviteCreateChanges Create(BaseDiscordClient discord, Model log, ChangeModel[] models)
        {
            //Again, FirstOrDefault to protect against ordering changes
            var maxAgeModel = models.FirstOrDefault(x => x.ChangedProperty == "max_age");
            var codeModel = models.FirstOrDefault(x => x.ChangedProperty == "code");
            var temporaryModel = models.FirstOrDefault(x => x.ChangedProperty == "temporary");
            var inviterIdModel = models.FirstOrDefault(x => x.ChangedProperty == "inviter_id");
            var channelIdModel = models.FirstOrDefault(x => x.ChangedProperty == "channel_id");
            var usesModel = models.FirstOrDefault(x => x.ChangedProperty == "uses");
            var maxUsesModel = models.FirstOrDefault(x => x.ChangedProperty == "max_uses");

            var maxAge = maxAgeModel.NewValue.ToObject<int>();
            var code = codeModel.NewValue.ToObject<string>();
            var temporary = temporaryModel.NewValue.ToObject<bool>();
            var inviterId = inviterIdModel.NewValue.ToObject<ulong>();
            var channelId = channelIdModel.NewValue.ToObject<ulong>();
            var uses = usesModel.NewValue.ToObject<int>();
            var maxUses = maxUsesModel.NewValue.ToObject<int>();

            var inviterInfo = log.Users.FirstOrDefault(x => x.Id == inviterId);
            var inviter = RestUser.Create(discord, inviterInfo);

            return new InviteCreateChanges(maxAge, code, temporary, inviter, channelId, uses, maxUses);
        }

        public int MaxAge { get; }
        public string Code { get; }
        public bool Temporary { get; }
        public IUser Creator { get; }
        public ulong ChannelId { get; } //TODO: IChannel-ify
        public int Uses { get; }
        public int MaxUses { get; }
    }
}
