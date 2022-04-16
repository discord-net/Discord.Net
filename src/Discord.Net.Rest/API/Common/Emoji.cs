using Newtonsoft.Json;

namespace Discord.API
{
    internal class Emoji : IEmojiModel
    {
        [JsonProperty("id")]
        public ulong? Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("animated")]
        public bool? Animated { get; set; }
        [JsonProperty("roles")]
        public ulong[] Roles { get; set; }
        [JsonProperty("require_colons")]
        public bool RequireColons { get; set; }
        [JsonProperty("managed")]
        public bool Managed { get; set; }
        [JsonProperty("available")]
        public Optional<bool> Available { get; set; }
        [JsonProperty("user")]
        public Optional<User> User { get; set; }

        ulong? IEmojiModel.Id => Id;

        string IEmojiModel.Name => Name;

        ulong[] IEmojiModel.Roles => Roles;

        bool IEmojiModel.RequireColons => RequireColons;

        bool IEmojiModel.IsManaged => Managed;

        bool IEmojiModel.IsAnimated => Animated.GetValueOrDefault();

        bool IEmojiModel.IsAvailable => Available.GetValueOrDefault();

        ulong? IEmojiModel.CreatorId => User.GetValueOrDefault()?.Id;
    }
}
