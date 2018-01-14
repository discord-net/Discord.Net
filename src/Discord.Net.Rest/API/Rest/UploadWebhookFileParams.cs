#pragma warning disable CS1591
using System.Collections.Generic;
using System.IO;
using Discord.Net.Rest;

namespace Discord.API.Rest
{
    internal class UploadWebhookFileParams
    {
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

            if (Content.IsSpecified)
                d["content"] = Content.Value;
            if (IsTTS.IsSpecified)
                d["tts"] = IsTTS.Value.ToString();
            if (Nonce.IsSpecified)
                d["nonce"] = Nonce.Value;
            if (Username.IsSpecified)
                d["username"] = Username.Value;
            if (AvatarUrl.IsSpecified)
                d["avatar_url"] = AvatarUrl.Value;
            if (Embeds.IsSpecified)
                d["embeds"] = Embeds.Value;
            return d;
        }
    }
}
