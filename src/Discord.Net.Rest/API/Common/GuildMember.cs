using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class GuildMember : IMemberModel
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("nick")]
        public Optional<string> Nick { get; set; }
        [JsonProperty("avatar")]
        public Optional<string> Avatar { get; set; }
        [JsonProperty("roles")]
        public Optional<ulong[]> Roles { get; set; }
        [JsonProperty("joined_at")]
        public Optional<DateTimeOffset> JoinedAt { get; set; }
        [JsonProperty("deaf")]
        public Optional<bool> Deaf { get; set; }
        [JsonProperty("mute")]
        public Optional<bool> Mute { get; set; }
        [JsonProperty("pending")]
        public Optional<bool> Pending { get; set; }
        [JsonProperty("premium_since")]
        public Optional<DateTimeOffset?> PremiumSince { get; set; }
        [JsonProperty("communication_disabled_until")]
        public Optional<DateTimeOffset?> TimedOutUntil { get; set; }

        // IMemberModel
        string IMemberModel.Nickname {
            get => Nick.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IMemberModel.GuildAvatar {
            get => Avatar.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        ulong[] IMemberModel.Roles {
            get => Roles.GetValueOrDefault(Array.Empty<ulong>()); set => throw new NotSupportedException();
        }

        DateTimeOffset? IMemberModel.JoinedAt {
            get => JoinedAt.ToNullable(); set => throw new NotSupportedException();
        }

        DateTimeOffset? IMemberModel.PremiumSince {
            get => PremiumSince.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        bool IMemberModel.IsDeaf {
            get => Deaf.GetValueOrDefault(false); set => throw new NotSupportedException();
        }

        bool IMemberModel.IsMute {
            get => Mute.GetValueOrDefault(false); set => throw new NotSupportedException();
        }

        bool? IMemberModel.IsPending {
            get => Pending.ToNullable(); set => throw new NotSupportedException();
        }

        DateTimeOffset? IMemberModel.CommunicationsDisabledUntil {
            get => TimedOutUntil.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        ulong IEntityModel<ulong>.Id {
            get => User.Id; set => throw new NotSupportedException();
        }
    }
}
