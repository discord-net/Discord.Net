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
    internal class CreateMultipartPostAsync
    {
        private static JsonSerializer _serializer = new JsonSerializer { ContractResolver = new DiscordContractResolver() };

        public FileAttachment[] Files { get; }

        public string Title { get; set; }
        public ThreadArchiveDuration ArchiveDuration { get; set; }
        public Optional<int?> Slowmode { get; set; }


        public Optional<string> Content { get; set; }
        public Optional<Embed[]> Embeds { get; set; }
        public Optional<AllowedMentions> AllowedMentions { get; set; }
        public Optional<ActionRowComponent[]> MessageComponent { get; set; }
        public Optional<MessageFlags?> Flags { get; set; }
        public Optional<ulong[]> Stickers { get; set; }

        public CreateMultipartPostAsync(params FileAttachment[] attachments)
        {
            Files = attachments;
        }

        public IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>();

            var payload = new Dictionary<string, object>();
            var message = new Dictionary<string, object>();

            payload["name"] = Title;
            payload["auto_archive_duration"] = ArchiveDuration;

            if (Slowmode.IsSpecified)
                payload["rate_limit_per_user"] = Slowmode.Value;

            // message
            if (Content.IsSpecified)
                message["content"] = Content.Value;
            if (Embeds.IsSpecified)
                message["embeds"] = Embeds.Value;
            if (AllowedMentions.IsSpecified)
                message["allowed_mentions"] = AllowedMentions.Value;
            if (MessageComponent.IsSpecified)
                message["components"] = MessageComponent.Value;
            if (Stickers.IsSpecified)
                message["sticker_ids"] = Stickers.Value;
            if (Flags.IsSpecified)
                message["flags"] = Flags.Value;

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

            message["attachments"] = attachments;

            payload["message"] = message;

            var json = new StringBuilder();
            using (var text = new StringWriter(json))
            using (var writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, payload);

            d["payload_json"] = json.ToString();

            return d;
        }
    }
}
