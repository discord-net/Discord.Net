using System;
using System.Diagnostics;
using System.Globalization;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct Emoji
    {
        public ulong? Id { get; }
        public string Name { get; }

        public string Url => Id != null ? CDN.GetEmojiUrl(Id.Value) : null;

        internal Emoji(ulong? id, string name)
        {
            Id = id;
            Name = name;
        }

        public static Emoji Parse(string text)
        {
            Emoji result;
            if (TryParse(text, out result))
                return result;
            throw new ArgumentException("Invalid emoji format", nameof(text));
        }

        public static bool TryParse(string text, out Emoji result)
        {
            result = default(Emoji);
            if (text.Length >= 4 && text[0] == '<' && text[1] == ':' && text[text.Length - 1] == '>')
            {
                int splitIndex = text.IndexOf(':', 2);
                if (splitIndex == -1)
                    return false;

                ulong id;
                if (!ulong.TryParse(text.Substring(splitIndex + 1, text.Length - splitIndex - 2), NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    return false;

                string name = text.Substring(2, splitIndex - 2);
                result = new Emoji(id, name);
                return true;
            }
            return false;

        }

        private string DebuggerDisplay => $"{Name} ({Id})";
        public override string ToString() => Name;
    }
}
