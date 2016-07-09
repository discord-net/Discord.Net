namespace Discord
{
    public interface IAttachment
    {
        ulong Id { get; }

        string Filename { get; }
        string Url { get; }
        string ProxyUrl { get; }
        int Size { get; }
        int? Height { get; }
        int? Width { get; }
    }
}
