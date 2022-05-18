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

        string IActivityModel.Id {
            get => Id.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.Url {
            get => StreamUrl.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.State {
            get => State.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        IEmojiModel IActivityModel.Emoji {
            get => Emoji.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.Name {
            get => Name; set => throw new NotSupportedException();
        }

        ActivityType IActivityModel.Type {
            get => Type.GetValueOrDefault().GetValueOrDefault(); set => throw new NotSupportedException();
        }

        ActivityProperties IActivityModel.Flags {
            get => Flags.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.Details {
            get => Details.GetValueOrDefault(); set => throw new NotSupportedException();
        }
        DateTimeOffset IActivityModel.CreatedAt {
            get => DateTimeOffset.FromUnixTimeMilliseconds(CreatedAt.GetValueOrDefault()); set => throw new NotSupportedException();
        }

        ulong? IActivityModel.ApplicationId {
            get => ApplicationId.ToNullable(); set => throw new NotSupportedException();
        }

        string IActivityModel.SyncId {
            get => SyncId.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.SessionId {
            get => SessionId.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.LargeImage {
            get => Assets.GetValueOrDefault()?.LargeImage.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.LargeText {
            get => Assets.GetValueOrDefault()?.LargeText.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.SmallImage {
            get => Assets.GetValueOrDefault()?.SmallImage.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.SmallText {
            get => Assets.GetValueOrDefault()?.SmallText.GetValueOrDefault(); set => throw new NotSupportedException();
        }

        string IActivityModel.PartyId {
            get => Party.GetValueOrDefault()?.Id; set => throw new NotSupportedException();
        }

        long[] IActivityModel.PartySize {
            get => Party.GetValueOrDefault()?.Size; set => throw new NotSupportedException();
        }

        string IActivityModel.JoinSecret {
            get => Secrets.GetValueOrDefault()?.Join; set => throw new NotSupportedException();
        }

        string IActivityModel.SpectateSecret {
            get => Secrets.GetValueOrDefault()?.Spectate; set => throw new NotSupportedException();
        }

        string IActivityModel.MatchSecret {
            get => Secrets.GetValueOrDefault()?.Match; set => throw new NotSupportedException();
        }

        DateTimeOffset? IActivityModel.TimestampStart {
            get => Timestamps.GetValueOrDefault()?.Start.ToNullable(); set => throw new NotSupportedException();
        }

        DateTimeOffset? IActivityModel.TimestampEnd {
            get => Timestamps.GetValueOrDefault()?.End.ToNullable(); set => throw new NotSupportedException();
        }



        //[JsonProperty("buttons")]
        //public Optional<RichPresenceButton[]> Buttons { get; set; }

        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }
    }
}
