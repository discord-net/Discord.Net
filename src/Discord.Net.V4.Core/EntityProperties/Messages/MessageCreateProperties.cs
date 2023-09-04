namespace Discord;

public sealed record MessageCreateProperties(
    Optional<string?> Content = default,
    Optional<bool> IsTTS = default,
    Optional<IEnumerable<Embed>> Embeds = default,
    Optional<AllowedMentions> AllowedMentions = default,
    Optional<MessageReference> Reference = default,
    // TODO: MessageComponent
    // TODO: Stickers
    // TODO: Attachments
    Optional<MessageFlags> Flags = default)
{
    internal bool IsValid()
    {
        // TODO: Validate
    }
}
