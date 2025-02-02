namespace Discord;

public readonly struct UnfurledMediaItem
{
    public string Url { get; }

    internal UnfurledMediaItem(string url)
    {
        Url = url;
    }
}
