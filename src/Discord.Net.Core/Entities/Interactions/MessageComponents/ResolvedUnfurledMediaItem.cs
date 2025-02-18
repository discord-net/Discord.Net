namespace Discord;

public class ResolvedUnfurledMediaItem : UnfurledMediaItem
{
    public string ProxyUrl { get; }

    public int Height { get; }

    public int Width { get; }

    public string ContentType { get;}

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
