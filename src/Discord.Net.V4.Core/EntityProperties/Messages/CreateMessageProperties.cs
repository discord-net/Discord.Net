namespace Discord;

public sealed record CreateMessageProperties(
    Optional<string?> Content = default,
    Optional<bool> IsTTS = default,
    Optional<IEnumerable<Embed>> Embeds = default,
    Optional<AllowedMentions> AllowedMentions = default,
    Optional<MessageReference> Reference = default,
    Optional<IEnumerable<IMessageComponent>> Components = default,
    Optional<IEnumerable<ulong>> Stickers = default,
    Optional<IEnumerable<FileAttachment>> Attachments = default,
    Optional<MessageFlags> Flags = default)
{
    internal bool IsValid() =>
        // TODO: Validate
        true;
}
