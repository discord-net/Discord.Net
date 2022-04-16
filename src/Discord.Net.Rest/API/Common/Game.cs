using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Runtime.Serialization;

namespace Discord.API
{
    internal class Game : IActivityModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public Optional<string> StreamUrl { get; set; }
        [JsonProperty("type")]
        public Optional<ActivityType?> Type { get; set; }
        [JsonProperty("details")]
        public Optional<string> Details { get; set; }
        [JsonProperty("state")]
        public Optional<string> State { get; set; }
        [JsonProperty("application_id")]
        public Optional<ulong> ApplicationId { get; set; }
        [JsonProperty("assets")]
        public Optional<API.GameAssets> Assets { get; set; }
        [JsonProperty("party")]
        public Optional<API.GameParty> Party { get; set; }
        [JsonProperty("secrets")]
        public Optional<API.GameSecrets> Secrets { get; set; }
        [JsonProperty("timestamps")]
        public Optional<API.GameTimestamps> Timestamps { get; set; }
        [JsonProperty("instance")]
        public Optional<bool> Instance { get; set; }
        [JsonProperty("sync_id")]
        public Optional<string> SyncId { get; set; }
        [JsonProperty("session_id")]
        public Optional<string> SessionId { get; set; }
        [JsonProperty("flags")]
        public Optional<ActivityProperties> Flags { get; set; }
        [JsonProperty("id")]
        public Optional<string> Id { get; set; }
        [JsonProperty("emoji")]
        public Optional<Emoji> Emoji { get; set; }
        [JsonProperty("created_at")]
        public Optional<long> CreatedAt { get; set; }

        string IActivityModel.Id => Id.GetValueOrDefault();

        string IActivityModel.Url => StreamUrl.GetValueOrDefault();

        string IActivityModel.State => State.GetValueOrDefault();

        IEmojiModel IActivityModel.Emoji => Emoji.GetValueOrDefault();

        string IActivityModel.Name => Name;

        ActivityType IActivityModel.Type => Type.GetValueOrDefault().GetValueOrDefault();

        ActivityProperties IActivityModel.Flags => Flags.GetValueOrDefault();

        string IActivityModel.Details => Details.GetValueOrDefault();
        DateTimeOffset IActivityModel.CreatedAt => DateTimeOffset.FromUnixTimeMilliseconds(CreatedAt.GetValueOrDefault());

        ulong? IActivityModel.ApplicationId => ApplicationId.ToNullable();

        string IActivityModel.SyncId => SyncId.GetValueOrDefault();

        string IActivityModel.SessionId => SessionId.GetValueOrDefault();

        string IActivityModel.LargeImage => Assets.GetValueOrDefault()?.LargeImage.GetValueOrDefault();

        string IActivityModel.LargeText => Assets.GetValueOrDefault()?.LargeText.GetValueOrDefault();

        string IActivityModel.SmallImage => Assets.GetValueOrDefault()?.SmallImage.GetValueOrDefault();

        string IActivityModel.SmallText => Assets.GetValueOrDefault()?.SmallText.GetValueOrDefault();

        string IActivityModel.PartyId => Party.GetValueOrDefault()?.Id;

        long[] IActivityModel.PartySize => Party.GetValueOrDefault()?.Size;

        string IActivityModel.JoinSecret => Secrets.GetValueOrDefault()?.Join;

        string IActivityModel.SpectateSecret => Secrets.GetValueOrDefault()?.Spectate;

        string IActivityModel.MatchSecret => Secrets.GetValueOrDefault()?.Match;

        DateTimeOffset? IActivityModel.TimestampStart => Timestamps.GetValueOrDefault()?.Start.ToNullable();

        DateTimeOffset? IActivityModel.TimestampEnd => Timestamps.GetValueOrDefault()?.End.ToNullable();



        //[JsonProperty("buttons")]
        //public Optional<RichPresenceButton[]> Buttons { get; set; }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }
    }
}
