using Newtonsoft.Json;

namespace Discord.API
{
    internal class StickerItem : IStickerItemModel
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("format_type")]
        public StickerFormatType FormatType { get; set; }


        ulong IStickerItemModel.Id { get => Id; set => throw new System.NotSupportedException(); }
        string IStickerItemModel.Name { get => Name; set => throw new System.NotSupportedException(); }
        StickerFormatType IStickerItemModel.Format { get => FormatType; set => throw new System.NotSupportedException(); }
    }
}
