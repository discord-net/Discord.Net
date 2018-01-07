namespace Discord
{
    public class GameAsset
    {
        internal GameAsset() { }

        internal ulong ApplicationId { get; set; }
        
        public string Text { get; internal set; }
        public string ImageId { get; internal set; }
        
        public string GetImageUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => CDN.GetRichAssetUrl(ApplicationId, ImageId, size, format);
    }
}