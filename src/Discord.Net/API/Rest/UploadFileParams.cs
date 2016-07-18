using Discord.Net.Rest;
using System.Collections.Generic;
using System.IO;

namespace Discord.API.Rest
{
    public class UploadFileParams
    {
        public Stream File { get; set; }
        public string Filename { get; set; } = "unknown.dat";

        public Optional<string> Content { get; set; }
        public Optional<string> Nonce { get; set; }
        public Optional<bool> IsTTS { get; set; }

        public UploadFileParams(Stream file)
        {
            File = file;
        }

        public IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>();
            d["file"] = new MultipartFile(File, Filename);
            if (Content.IsSpecified)
                d["content"] = Content.Value;
            if (IsTTS.IsSpecified)
                d["tts"] = IsTTS.Value.ToString();
            if (Nonce.IsSpecified)
                d["nonce"] = Nonce.Value;
            return d;
        }
    }
}
