namespace Discord
{
    public interface IAttachment
    {
        /// <summary> The snowflake ID of the attachment. </summary>
        ulong Id { get; }

        /// <summary> The filename of the attachment. </summary>
        string Filename { get; }
        /// <summary> The URL of the attachment. </summary>
        string Url { get; }
        /// <summary> The proxied URL of the attachment. </summary>
        string ProxyUrl { get; }
        /// <summary> The file size of the attachment. </summary>
        int Size { get; }
        /// <summary> The height of the attachment if it is an image, or return <see langword="null"/> when it is not. </summary>
        int? Height { get; }
        /// <summary> The width of the attachment if it is an image, or return <see langword="null"/> when it is not. </summary>
        int? Width { get; }
    }
}
