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
        bool? ICurrentUserModel.IsVerified
        {
            get => Verified.ToNullable();
            set => throw new NotSupportedException();
        }

        string ICurrentUserModel.Email
        {
            get => Email.GetValueOrDefault();
            set => throw new NotSupportedException();
        }

        bool? ICurrentUserModel.IsMfaEnabled
        {
            get => MfaEnabled.ToNullable();
            set => throw new NotSupportedException();
        }

        UserProperties ICurrentUserModel.Flags
        {
            get => Flags.GetValueOrDefault();
            set => throw new NotSupportedException();
        }

        PremiumType ICurrentUserModel.PremiumType
        {
            get => PremiumType.GetValueOrDefault();
            set => throw new NotSupportedException();
        }

        string ICurrentUserModel.Locale
        {
            get => Locale.GetValueOrDefault();
            set => throw new NotSupportedException();
        }

        UserProperties ICurrentUserModel.PublicFlags
        {
            get => PublicFlags.GetValueOrDefault();
            set => throw new NotSupportedException();
        }
    }
}
