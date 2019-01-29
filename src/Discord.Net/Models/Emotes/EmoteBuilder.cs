using System;

namespace Discord
{
    public static class EmoteBuilder
    {
        public static IEmote FromEmoji(string emoji)
            => new Emoji(emoji);
        public static IEmote FromMention(string mention)
            => throw new NotImplementedException(); // TODO: emoteutil
        public static IEmote FromID(ulong id, string name)
            => new Emote(id, name);
    }
}
