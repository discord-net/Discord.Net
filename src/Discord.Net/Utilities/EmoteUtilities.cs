using System;

namespace Discord
{
    public static class EmoteUtilities
    {
        public static string FormatGuildEmote(ulong id, string name)
            => $"<:{name}:{id}>";

        public static (ulong, string) ParseGuildEmote(string formatted)
        {
            if (formatted.IndexOf('<') != 0 || formatted.IndexOf(':') != 1 || formatted.IndexOf('>') != formatted.Length-1)
                throw new ArgumentException("passed string does not match a guild emote format", nameof(formatted)); // TODO: grammar

            int closingIndex = formatted.IndexOf(':', 2);
            if (closingIndex < 0)
                throw new ArgumentException("passed string does not match a guild emote format", nameof(formatted));

            string name = formatted.Substring(2, closingIndex-2);
            string idStr = formatted.Substring(closingIndex + 1);
            idStr = idStr.Substring(0, idStr.Length - 1); // ignore closing >
            ulong id = ulong.Parse(idStr); // TODO: TryParse here?

            return (id, name);
        }
    }
}
