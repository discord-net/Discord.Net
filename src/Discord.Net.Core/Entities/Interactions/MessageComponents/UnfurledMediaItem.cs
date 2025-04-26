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

    internal UnfurledMediaItem(string url)
    {
        Url = url;
    }
}
