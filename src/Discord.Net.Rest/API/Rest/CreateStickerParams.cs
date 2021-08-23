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
    internal class CreateStickerParams
    {
        public Stream File { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }

        public IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>();

            d["file"] = new MultipartFile(File, Name + ".dat");

            d["name"] = Name;
            d["description"] = Description;
            d["tags"] = Tags;

            return d;
        }
    }
}
