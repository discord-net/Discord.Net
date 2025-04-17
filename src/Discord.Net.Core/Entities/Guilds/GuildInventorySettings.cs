namespace Discord;

public struct GuildInventorySettings
{
    /// <summary>
    ///     Gets whether everyone can collect the pack to use emojis across servers.
    /// </summary>
    public bool IsEmojiPackCollectible { get; }

    internal GuildInventorySettings(bool isEmojiPackCollectible)
    {
        IsEmojiPackCollectible = isEmojiPackCollectible;
    }
}
