#pragma warning disable CS1591
using Discord.Net.Rest;
using System.Collections.Generic;
using System.IO;

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
            return d;
        }
    }
}
