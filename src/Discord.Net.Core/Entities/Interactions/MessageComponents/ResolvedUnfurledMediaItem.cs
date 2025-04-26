namespace Discord;

/// <summary>
///     Represents a media item that has been unfurled and resolved.
/// </summary>
public class ResolvedUnfurledMediaItem : UnfurledMediaItem
{
    /// <summary>
    ///     Gets the proxy URL for this media item.
    /// </summary>
    public string ProxyUrl { get; }

    /// <summary>
    ///     Gets the height of this media item.
    /// </summary>
    public int Height { get; }

    /// <summary>
    ///     Gets the width of this media item.
    /// </summary>
    public int Width { get; }

    /// <summary>
    ///     Gets the content type of this media item.
    /// </summary>
    public string ContentType { get;}

    /// <summary>
    ///     Gets the loading state of this media item.
    /// </summary>
    public UnfurledMediaItemLoadingState LoadingState { get; }

    internal ResolvedUnfurledMediaItem(string url, string proxyUrl, int height, int width, string contentType, UnfurledMediaItemLoadingState loadingState) : base(url)
    {
        ProxyUrl = proxyUrl;
        Height = height;
        Width = width;
        ContentType = contentType;
        LoadingState = loadingState;
    }
}
