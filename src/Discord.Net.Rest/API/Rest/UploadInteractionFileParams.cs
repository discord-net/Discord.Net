using Discord.Net.Converters;
using Discord.Net.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class UploadInteractionFileParams
    {
        private static JsonSerializer _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };

        public FileAttachment[] Files { get; }

        public InteractionResponseType Type { get; set; }
        public Optional<string> Content { get; set; }
        public Optional<bool> IsTTS { get; set; }
        public Optional<Embed[]> Embeds { get; set; }
        public Optional<AllowedMentions> AllowedMentions { get; set; }
        public Optional<ActionRowComponent[]> MessageComponents { get; set; }
        public Optional<MessageFlags> Flags { get; set; }

        public bool HasData
            => Content.IsSpecified ||
               IsTTS.IsSpecified ||
               Embeds.IsSpecified ||
               AllowedMentions.IsSpecified ||
               MessageComponents.IsSpecified ||
               Flags.IsSpecified ||
               Files.Any();

        public UploadInteractionFileParams(params FileAttachment[] files)
        {
            Files = files;
        }

        public IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>();
           

            var payload = new Dictionary<string, object>();
            payload["type"] = Type;

            var data = new Dictionary<string, object>();
            if (Content.IsSpecified)
                data["content"] = Content.Value;
            if (IsTTS.IsSpecified)
                data["tts"] = IsTTS.Value;
            if (MessageComponents.IsSpecified)
                data["components"] = MessageComponents.Value;
            if (Embeds.IsSpecified)
                data["embeds"] = Embeds.Value;
            if (AllowedMentions.IsSpecified)
                data["allowed_mentions"] = AllowedMentions.Value;
            if (Flags.IsSpecified)
                data["flags"] = Flags.Value;

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

            data["attachments"] = attachments;

            payload["data"] = data;


            if (data.Any())
            {
                var json = new StringBuilder();
                using (var text = new StringWriter(json))
                using (var writer = new JsonTextWriter(text))
                    _serializer.Serialize(writer, payload);
                d["payload_json"] = json.ToString();
            }

            return d;
        }
    }
}
