using System.Collections.Generic;

namespace Discord.API.Rest
{
    public class UploadFileParams
    {
        public string Content { get; set; } = "";
        public string Nonce { get; set; } = null;
        public bool IsTTS { get; set; } = false;
        public string Filename { get; set; } = "unknown.dat";

        public IReadOnlyDictionary<string, string> ToDictionary()
        {
            var dic = new Dictionary<string, string>
            {
                ["content"] = Content,
                ["tts"] = IsTTS.ToString()
            };
            if (Nonce != null)
                dic.Add("nonce", Nonce);
            return dic;
        }
    }
}
