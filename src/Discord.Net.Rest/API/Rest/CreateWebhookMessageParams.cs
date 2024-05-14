using Discord.Net.Converters;
using Discord.Net.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Discord.API.Rest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class CreateWebhookMessageParams
    {
        private static JsonSerializer _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };

        [JsonProperty("content")]
        public Optional<string> Content { get; set; }

        [JsonProperty("nonce")]
        public Optional<string> Nonce { get; set; }

        [JsonProperty("tts")]
        public Optional<bool> IsTTS { get; set; }

        [JsonProperty("embeds")]
        public Optional<Embed[]> Embeds { get; set; }

        [JsonProperty("username")]
        public Optional<string> Username { get; set; }

        [JsonProperty("avatar_url")]
        public Optional<string> AvatarUrl { get; set; }

        [JsonProperty("allowed_mentions")]
        public Optional<AllowedMentions> AllowedMentions { get; set; }

        [JsonProperty("flags")]
        public Optional<MessageFlags> Flags { get; set; }

        [JsonProperty("components")]
        public Optional<API.ActionRowComponent[]> Components { get; set; }

        [JsonProperty("file")]
        public Optional<MultipartFile> File { get; set; }

        [JsonProperty("thread_name")]
        public Optional<string> ThreadName { get; set; }

        [JsonProperty("applied_tags")]
        public Optional<ulong[]> AppliedTags { get; set; }

        [JsonProperty("poll")]
        public Optional<CreatePollParams> Poll { get; set; }

        public IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>();

            if (File.IsSpecified)
            {
                d["file"] = File.Value;
            }

            var payload = new Dictionary<string, object>
            {
                ["content"] = Content
            };

            if (IsTTS.IsSpecified)
                payload["tts"] = IsTTS.Value.ToString();
            if (Nonce.IsSpecified)
                payload["nonce"] = Nonce.Value;
            if (Username.IsSpecified)
                payload["username"] = Username.Value;
            if (AvatarUrl.IsSpecified)
                payload["avatar_url"] = AvatarUrl.Value;
            if (Embeds.IsSpecified)
                payload["embeds"] = Embeds.Value;
            if (AllowedMentions.IsSpecified)
                payload["allowed_mentions"] = AllowedMentions.Value;
            if (Components.IsSpecified)
                payload["components"] = Components.Value;
            if (ThreadName.IsSpecified)
                payload["thread_name"] = ThreadName.Value;
            if (AppliedTags.IsSpecified)
                payload["applied_tags"] = AppliedTags.Value;
            if (Flags.IsSpecified)
                payload["flags"] = Flags.Value;
            if (Poll.IsSpecified)
                payload["poll"] = Poll.Value;


            var json = new StringBuilder();
            using (var text = new StringWriter(json))
            using (var writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, payload);

            d["payload_json"] = json.ToString();

            return d;
        }
    }
}
