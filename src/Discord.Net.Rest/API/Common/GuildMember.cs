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
        string IMemberModel.Nickname => Nick.GetValueOrDefault();

        string IMemberModel.GuildAvatar => Avatar.GetValueOrDefault();

        ulong[] IMemberModel.Roles => Roles.GetValueOrDefault(Array.Empty<ulong>());

        DateTimeOffset IMemberModel.JoinedAt => JoinedAt.GetValueOrDefault();

        DateTimeOffset? IMemberModel.PremiumSince =>  PremiumSince.GetValueOrDefault();

        bool IMemberModel.IsDeaf => Deaf.GetValueOrDefault(false);

        bool IMemberModel.IsMute => Mute.GetValueOrDefault(false);

        bool? IMemberModel.IsPending => Pending.ToNullable();

        DateTimeOffset? IMemberModel.CommunicationsDisabledUntil => TimedOutUntil.GetValueOrDefault();

        IUserModel IMemberModel.User => User;
    }
}
