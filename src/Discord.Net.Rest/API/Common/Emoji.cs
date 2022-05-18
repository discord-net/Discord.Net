using Newtonsoft.Json;
using System;

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

        ulong? IEmojiModel.Id
        {
            get => Id;
            set => throw new NotSupportedException();
        }

        string IEmojiModel.Name
        {
            get => Name;
            set => throw new NotSupportedException();
        }

        ulong[] IEmojiModel.Roles
        {
            get => Roles;
            set => throw new NotSupportedException();
        }

        bool IEmojiModel.RequireColons
        {
            get => RequireColons;
            set => throw new NotSupportedException();
        }

        bool IEmojiModel.IsManaged
        {
            get => Managed;
            set => throw new NotSupportedException();
        }

        bool IEmojiModel.IsAnimated
        {
            get => Animated.GetValueOrDefault();
            set => throw new NotSupportedException();
        }

        bool IEmojiModel.IsAvailable
        {
            get => Available.GetValueOrDefault();
            set => throw new NotSupportedException();
        }

        ulong? IEmojiModel.CreatorId
        {
            get => User.GetValueOrDefault()?.Id;
            set => throw new NotSupportedException();
        }
    }
}
