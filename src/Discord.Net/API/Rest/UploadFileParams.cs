#pragma warning disable CS1591
using Discord.Net.Rest;
using System.Collections.Generic;
using System.IO;

namespace Discord.API.Rest
{
    public class UploadFileParams
    {
        public Stream File { internal get; set; }

        internal Optional<string> _filename { get; set; }
        public string Filename { set { _filename = value; } }

        internal Optional<string> _content { get; set; }
        public string Content { set { _content = value; } }

        internal Optional<string> _nonce { get; set; }
        public string Nonce { set { _nonce = value; } }

        internal Optional<bool> _isTTS { get; set; }
        public bool IsTTS { set { _isTTS = value; } }

        public UploadFileParams(Stream file)
        {
            File = file;
        }

        internal IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>();
            d["file"] = new MultipartFile(File, _filename.GetValueOrDefault("unknown.dat"));
            if (_content.IsSpecified)
                d["content"] = _content.Value;
            if (_isTTS.IsSpecified)
                d["tts"] = _isTTS.Value.ToString();
            if (_nonce.IsSpecified)
                d["nonce"] = _nonce.Value;
            return d;
        }
    }
}
