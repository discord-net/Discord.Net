using Discord.Net.Rest;
using System.Collections.Generic;
using System.IO;
namespace Discord.API.Rest
{
    internal class CreateStickerParams
    {
        public Stream File { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string FileName { get; set; }

        public IReadOnlyDictionary<string, object> ToDictionary()
        {
            var d = new Dictionary<string, object>
            {
                ["name"] = $"{Name}",
                ["description"] = Description,
                ["tags"] = Tags
            };

            string contentType = "image/png";

            if (File is FileStream fileStream)
                contentType = $"image/{Path.GetExtension(fileStream.Name)}";
            else if (FileName != null)
                contentType = $"image/{Path.GetExtension(FileName)}";

            d["file"] = new MultipartFile(File, FileName ?? "image", contentType.Replace(".", ""));

            return d;
        }
    }
}
