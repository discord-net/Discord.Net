namespace Discord
{
    public interface IEmbed
    {
        string Url { get; }
        string Type { get; }
        string Title { get; }
        string Description { get; }
        EmbedProvider Provider { get; }
        EmbedThumbnail Thumbnail { get; }
    }
}
