namespace Discord
{
    /// <summary> Represents a Discord attachment. </summary>
    public interface IAttachment
    {
        /// <summary> Gets the snowflake ID of the attachment. </summary>
        ulong Id { get; }

        /// <summary> Gets the filename of the attachment. </summary>
        string Filename { get; }
        /// <summary> Gets the URL of the attachment. </summary>
        string Url { get; }
        /// <summary> Gets the proxied URL of the attachment. </summary>
        string ProxyUrl { get; }
        /// <summary> Gets the file size of the attachment. </summary>
        int Size { get; }
        /// <summary> Gets the height of the attachment if it is an image, or return <see langword="null"/> when it is not. </summary>
        int? Height { get; }
        /// <summary> Gets the width of the attachment if it is an image, or return <see langword="null"/> when it is not. </summary>
        int? Width { get; }
    }
}
