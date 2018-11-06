#pragma warning disable CS1591
using System.Collections.Generic;
using System.IO;
using System.Text;
using Discord.Net.Converters;
using Discord.Net.Rest;
using Newtonsoft.Json;

namespace Discord.API.Rest
{
    internal class UploadWebhookFileParams
    {
        private static JsonSerializer _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };

        public Stream File { get; }

        public Optional<string> Filename { get; set; }
        public Optional<string> Content { get; set; }
        public Optional<string> Nonce { get; set; }
        public Optional<bool> IsTTS { get; set; }
        public Optional<string> Username { get; set; }
        public Optional<string> AvatarUrl { get; set; }
        public Optional<Embed[]> Embeds { get; set; }

        public UploadWebhookFileParams(Stream file)
        {
            File = file;
        }

        public IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>();
            d["file"] = new MultipartFile(File, Filename.GetValueOrDefault("unknown.dat"));

            var payload = new Dictionary<string, object>();
            if (Content.IsSpecified)
                payload["content"] = Content.Value;
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

            var json = new StringBuilder();
            using (var text = new StringWriter(json))
            using (var writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, payload);

            d["payload_json"] = json.ToString();

            return d;
        }
    }
}
