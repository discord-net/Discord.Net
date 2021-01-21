#pragma warning disable CS1591
using Discord.Net.Converters;
using Discord.Net.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Discord.API.Rest
{
    internal class UploadFileParams
    {
        private static JsonSerializer _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };

        public Stream File { get; }

        public Optional<string> Filename { get; set; }
        public Optional<string> Content { get; set; }
        public Optional<string> Nonce { get; set; }
        public Optional<bool> IsTTS { get; set; }
        public Optional<Embed> Embed { get; set; }
        public Optional<AllowedMentions> AllowedMentions { get; set; }
        public Optional<MessageReference> MessageReference { get; set; }
        public bool IsSpoiler { get; set; } = false;

        public UploadFileParams(Stream file)
        {
            File = file;
        }

        public IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>();
            var filename = Filename.GetValueOrDefault("unknown.dat");
            if (IsSpoiler && !filename.StartsWith(AttachmentExtensions.SpoilerPrefix))
                filename = filename.Insert(0, AttachmentExtensions.SpoilerPrefix);
            d["file"] = new MultipartFile(File, filename);

            var payload = new Dictionary<string, object>();
            if (Content.IsSpecified)
                payload["content"] = Content.Value;
            if (IsTTS.IsSpecified)
                payload["tts"] = IsTTS.Value.ToString();
            if (Nonce.IsSpecified)
                payload["nonce"] = Nonce.Value;
            if (Embed.IsSpecified)
                payload["embed"] = Embed.Value;
            if (AllowedMentions.IsSpecified)
                payload["allowed_mentions"] = AllowedMentions.Value;
            if (IsSpoiler)
                payload["hasSpoiler"] = IsSpoiler.ToString();
            if (MessageReference.IsSpecified)
                payload["message_reference"] = MessageReference.Value;

            var json = new StringBuilder();
            using (var text = new StringWriter(json))
            using (var writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, payload);

            d["payload_json"] = json.ToString();

            return d;
        }
    }
}
