namespace Discord.API.Rest;

internal class ListEntitlementsParams
{
    public Optional<ulong> UserId { get; set; }

    public Optional<ulong[]> SkuIds { get; set; }

    public Optional<ulong> BeforeId { get; set; }

    public Optional<ulong> AfterId { get; set; }

    public Optional<int> Limit { get; set; }

    public Optional<ulong> GuildId { get; set; }

    public Optional<bool> ExcludeEnded { get; set; }

    public Optional<bool> ExcludeDeleted { get; set; }
}
