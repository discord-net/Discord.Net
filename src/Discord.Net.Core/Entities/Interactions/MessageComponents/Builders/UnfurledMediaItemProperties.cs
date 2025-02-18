namespace Discord;

public struct UnfurledMediaItemProperties
{
    /// <summary>
    ///     Gets or sets the URL of the media item.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnfurledMediaItemProperties"/>.
    /// </summary>
    public UnfurledMediaItemProperties() {}

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnfurledMediaItemProperties"/>.
    /// </summary>
    public UnfurledMediaItemProperties(string url)
    {
        Url = url;
    }

    public static implicit operator UnfurledMediaItemProperties(string url) => new(url);
}
