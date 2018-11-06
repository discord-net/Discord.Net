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
        /// <returns>
        ///     A string containing the description of the asset.
        /// </returns>
        public string Text { get; internal set; }
        /// <summary>
        ///     Gets the image ID of the asset.
        /// </summary>
        /// <returns>
        ///     A string containing the unique image identifier of the asset.
        /// </returns>
        public string ImageId { get; internal set; }

        /// <summary>
        ///     Returns the image URL of the asset.
        /// </summary>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A string pointing to the image URL of the asset; <c>null</c> when the application ID does not exist.
        /// </returns>
        public string GetImageUrl(ImageFormat format = ImageFormat.Auto, ushort size = 128)
            => ApplicationId.HasValue ? CDN.GetRichAssetUrl(ApplicationId.Value, ImageId, size, format) : null;
    }
}
