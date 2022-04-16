using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class CurrentUser : User, ICurrentUserModel
    {
        [JsonProperty("verified")]
        public Optional<bool> Verified { get; set; }
        [JsonProperty("email")]
        public Optional<string> Email { get; set; }
        [JsonProperty("mfa_enabled")]
        public Optional<bool> MfaEnabled { get; set; }
        [JsonProperty("flags")]
        public Optional<UserProperties> Flags { get; set; }
        [JsonProperty("premium_type")]
        public Optional<PremiumType> PremiumType { get; set; }
        [JsonProperty("locale")]
        public Optional<string> Locale { get; set; }
        [JsonProperty("public_flags")]
        public Optional<UserProperties> PublicFlags { get; set; }

        // ICurrentUserModel
        bool? ICurrentUserModel.IsVerified => Verified.ToNullable();

        string ICurrentUserModel.Email => Email.GetValueOrDefault();

        bool? ICurrentUserModel.IsMfaEnabled => MfaEnabled.ToNullable();

        UserProperties ICurrentUserModel.Flags => Flags.GetValueOrDefault();

        PremiumType ICurrentUserModel.PremiumType => PremiumType.GetValueOrDefault();

        string ICurrentUserModel.Locale => Locale.GetValueOrDefault();

        UserProperties ICurrentUserModel.PublicFlags => PublicFlags.GetValueOrDefault();
    }
}
