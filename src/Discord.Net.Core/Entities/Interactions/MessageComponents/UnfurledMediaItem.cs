namespace Discord;

public class UnfurledMediaItem
{
    public string Url { get; }

    internal UnfurledMediaItem(string url)
    {
        Url = url;
    }
}
