namespace Discord;

/// <summary>
///     Represents a media item that has been unfurled.
/// </summary>
public class UnfurledMediaItem
{
    /// <summary>
    ///    Gets the URL of this media item.
    /// </summary>
    public string Url { get; }

    /// <summary>
    ///     Converts a <see cref="UnfurledMediaItem"/> to a <see cref="UnfurledMediaItemProperties"/>.
    /// </summary>
    public UnfurledMediaItemProperties ToProperties()
        => new(Url);

    internal UnfurledMediaItem(string url)
    {
        Url = url;
    }
}
