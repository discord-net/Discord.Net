using Discord.Models;

namespace Discord;

public readonly struct PollMedia(string? text, DiscordEmojiId? emoji) : 
    IModelConstructable<PollMedia, IPollMediaModel>
{
    public string? Text { get; } = text;
    public DiscordEmojiId? Emoji { get; } = emoji;

    public static PollMedia Construct(IDiscordClient client, IPollMediaModel model)
        => new(model.Text, model.Emoji);
}