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

        public FileAttachment[] Files { get; }

        public Optional<string> Content { get; set; }
        public Optional<string> Nonce { get; set; }
        public Optional<bool> IsTTS { get; set; }
        public Optional<string> Username { get; set; }
        public Optional<string> AvatarUrl { get; set; }
        public Optional<Embed[]> Embeds { get; set; }
        public Optional<AllowedMentions> AllowedMentions { get; set; }
        public Optional<ActionRowComponent[]> MessageComponents { get; set; }
        public Optional<MessageFlags> Flags { get; set; }

        public UploadWebhookFileParams(params FileAttachment[] files)
        {
            Files = files;
        }

        public IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>();

            var payload = new Dictionary<string, object>();
            if (Content.IsSpecified)
                payload["content"] = Content.Value;
            if (IsTTS.IsSpecified)
                payload["tts"] = IsTTS.Value;
            if (Nonce.IsSpecified)
                payload["nonce"] = Nonce.Value;
            if (Username.IsSpecified)
                payload["username"] = Username.Value;
            if (AvatarUrl.IsSpecified)
                payload["avatar_url"] = AvatarUrl.Value;
            if (MessageComponents.IsSpecified)
                payload["components"] = MessageComponents.Value;
            if (Embeds.IsSpecified)
                payload["embeds"] = Embeds.Value;
            if (AllowedMentions.IsSpecified)
                payload["allowed_mentions"] = AllowedMentions.Value;
            if (Flags.IsSpecified)
                payload["flags"] = Flags.Value;

            List<object> attachments = new();

            for (int n = 0; n != Files.Length; n++)
            {
                var attachment = Files[n];

                var filename = attachment.FileName ?? "unknown.dat";
                if (attachment.IsSpoiler && !filename.StartsWith(AttachmentExtensions.SpoilerPrefix))
                    filename = filename.Insert(0, AttachmentExtensions.SpoilerPrefix);
                d[$"files[{n}]"] = new MultipartFile(attachment.Stream, filename);

                attachments.Add(new
                {
                    id = (ulong)n,
                    filename = filename,
                    description = attachment.Description ?? Optional<string>.Unspecified
                });
            }

            payload["attachments"] = attachments;

            var json = new StringBuilder();
            using (var text = new StringWriter(json))
            using (var writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, payload);

            d["payload_json"] = json.ToString();

            return d;
        }
    }
}
