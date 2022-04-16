using Newtonsoft.Json;

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
        string IUserModel.Username => Username.GetValueOrDefault();

        string IUserModel.Discriminator => Discriminator.GetValueOrDefault();

        bool? IUserModel.IsBot => Bot.ToNullable();

        string IUserModel.Avatar => Avatar.GetValueOrDefault();

        ulong IEntity<ulong>.Id => Id;
    }
}
