using System;

namespace Discord
{
    public static class EmoteUtilities
    {
        public static string FormatGuildEmote(ulong id, string name)
            => $"<:{name}:{id}>";

        // TODO: perf: bench whether this should be passed by ref (in)
        public static bool TryParseGuildEmote(ReadOnlySpan<char> formatted, out (ulong, string) result)
        {
            result = default;

            if (formatted.IndexOf('<') != 0 || formatted.IndexOf(':') != 1 || formatted.IndexOf('>') != formatted.Length - 1)
                return false;

            int closingIndex = formatted.LastIndexOf(':');
            if (closingIndex < 0)
                return false;

            ReadOnlySpan<char> name = formatted.Slice(2, closingIndex-2);
            ReadOnlySpan<char> idStr = formatted.Slice(closingIndex + 1, formatted.Length - (name.Length + 4));
            idStr = idStr.Slice(0, idStr.Length - 1); // ignore closing >

            if (!ulong.TryParse(idStr.ToString(), out ulong id))
                return false;

            result = (id, name.ToString());

            return true;
        }
    }
}
