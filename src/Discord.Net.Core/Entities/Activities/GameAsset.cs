namespace Discord
{
    /// <summary>
    ///     An asset for a <see cref="RichGame" /> object containing the text and image.
    /// </summary>
    public class GameAsset
    {
        internal GameAsset() { }

        internal ulong? ApplicationId { get; set; }

        /// <summary>
        ///     Gets the description of the asset.
        /// </summary>
        public string Text { get; internal set; }
        /// <summary>
        ///     Gets the image ID of the asset.
        /// </summary>
        public string ImageId { get; internal set; }

        /// <summary>
        ///     Returns the image URL of the asset, or <c>null</c> when the application ID does not exist.
        /// </summary>
        public string GetImageUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => ApplicationId.HasValue ? CDN.GetRichAssetUrl(ApplicationId.Value, ImageId, size, format) : null;
    }
}
