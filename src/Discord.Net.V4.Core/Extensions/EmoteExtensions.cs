using Discord.Models.Json;

namespace Discord;

public static class EmoteExtensions
{
    public static DefaultReaction ToDefaultReactionModel(this IEmote emote)
    {
        switch (emote)
        {
            case IGuildEmote guildEmote:
                return new DefaultReaction {Id = guildEmote.Id};
            default:
                return new DefaultReaction {Name = emote.Name};
        }
    }
}
