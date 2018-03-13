#pragma warning disable CS1591
using Discord.Net.Converters;
using Discord.Net.Rest;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
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

        public UploadFileParams(Stream file)
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
            if (Embed.IsSpecified)
            {
                var payload = new StringBuilder();
                using (var text = new StringWriter(payload))
                using (var writer = new JsonTextWriter(text))
                {
                    var map = new Dictionary<string, object>()
                    {
                        ["embed"] = Embed.Value,
                    };

                    _serializer.Serialize(writer, map);
                }
                d["payload_json"] = payload.ToString();

            }
            return d;
        }
    }
}
