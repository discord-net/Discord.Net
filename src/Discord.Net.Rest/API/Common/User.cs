using Newtonsoft.Json;
using System;

namespace Discord.API
{
    internal class User : IUserModel
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("username")]
        public Optional<string> Username { get; set; }
        [JsonProperty("discriminator")]
        public Optional<string> Discriminator { get; set; }
        [JsonProperty("bot")]
        public Optional<bool> Bot { get; set; }
        [JsonProperty("avatar")]
        public Optional<string> Avatar { get; set; }
        [JsonProperty("banner")]
        public Optional<string> Banner { get; set; }
        [JsonProperty("accent_color")]
        public Optional<uint?> AccentColor { get; set; }


        // IUserModel
        string IUserModel.Username
        {
            get => Username.GetValueOrDefault();
            set => throw new NotSupportedException();
        }

        string IUserModel.Discriminator {
            get => Discriminator.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        bool? IUserModel.IsBot
        {
            get => Bot.ToNullable();
            set => throw new NotSupportedException();
        }

        string IUserModel.Avatar
        {
            get => Avatar.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        ulong IEntityModel<ulong>.Id
        {
            get => Id;
            set => throw new NotSupportedException();
        }
    }
}
