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
                ["tags"] = Tags
            };

            if (Description is not null)
                d["description"] = Description;
            else
                d["description"] = string.Empty;

            string contentType;
            if (File is FileStream fileStream)
            {
                var extension = Path.GetExtension(fileStream.Name).TrimStart('.');
                contentType = extension == "json" ? "application/json" : $"image/{extension}";
            }
            else if (FileName != null)
            {
                var extension = Path.GetExtension(FileName).TrimStart('.');
                contentType = extension == "json" ? "application/json" : $"image/{extension}";
            }
            else
                contentType = "image/png";

            d["file"] = new MultipartFile(File, FileName ?? "image", contentType);

            return d;
        }
    }
}
