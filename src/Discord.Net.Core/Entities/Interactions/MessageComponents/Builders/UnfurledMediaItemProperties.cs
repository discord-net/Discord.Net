namespace Discord;

public struct UnfurledMediaItemProperties
{
    public string Url { get; set; }

    public UnfurledMediaItemProperties() {}
    public UnfurledMediaItemProperties(string url)
    {
        Url = url;
    }

    public static implicit operator UnfurledMediaItemProperties(string url) => new(url);
}
