using System.Linq;

using Model = Discord.API.AuditLog;
using EntryModel = Discord.API.AuditLogEntry;
using ChangeModel = Discord.API.AuditLogChange;

namespace Discord.Rest
{
    public class MemberUpdateAuditLogData : IAuditLogData
    {
        private MemberUpdateAuditLogData(IUser target, MemberInfo before, MemberInfo after)
        {
            Target = target;
            Before = before;
            After = after;
        }

        internal static MemberUpdateAuditLogData Create(BaseDiscordClient discord, Model log, EntryModel entry)
        {
            var changes = entry.Changes;

            var nickModel = changes.FirstOrDefault(x => x.ChangedProperty == "nick");
            var deafModel = changes.FirstOrDefault(x => x.ChangedProperty == "deaf");
            var muteModel = changes.FirstOrDefault(x => x.ChangedProperty == "mute");
            var avatarModel = changes.FirstOrDefault(x => x.ChangedProperty == "avatar_hash");

            string oldNick = nickModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newNick = nickModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);
            bool? oldDeaf = deafModel?.OldValue?.ToObject<bool>(discord.ApiClient.Serializer),
                newDeaf = deafModel?.NewValue?.ToObject<bool>(discord.ApiClient.Serializer);
            bool? oldMute = muteModel?.OldValue?.ToObject<bool>(discord.ApiClient.Serializer),
                newMute = muteModel?.NewValue?.ToObject<bool>(discord.ApiClient.Serializer);
            string oldAvatar = avatarModel?.OldValue?.ToObject<string>(discord.ApiClient.Serializer),
                newAvatar = avatarModel?.NewValue?.ToObject<string>(discord.ApiClient.Serializer);

            var targetInfo = log.Users.FirstOrDefault(x => x.Id == entry.TargetId);
            var user = RestUser.Create(discord, targetInfo);

            var before = new MemberInfo(oldNick, oldDeaf, oldMute, oldAvatar);
            var after = new MemberInfo(newNick, newDeaf, newMute, newAvatar);

            return new MemberUpdateAuditLogData(user, before, after);
        }

        public IUser Target { get; }
        public MemberInfo Before { get; }
        public MemberInfo After { get; }
    }
}
