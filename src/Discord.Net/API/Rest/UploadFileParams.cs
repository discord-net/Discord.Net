using System.Collections.Generic;

namespace Discord.API.Rest
{
    public class UploadFileParams
    {
        public string Filename { get; set; } = "unknown.dat";

        public Optional<string> Content { get; set; }
        public Optional<string> Nonce { get; set; }
        public Optional<bool> IsTTS { get; set; }

        public IReadOnlyDictionary<string, string> ToDictionary()
        {
            var d = new Dictionary<string, string>();
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
