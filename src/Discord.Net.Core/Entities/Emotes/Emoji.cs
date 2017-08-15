using System;
using System.Text;

namespace Discord
{
    /// <summary>
    /// A unicode emoji
    /// </summary>
    public partial class Emoji : IEmote
    {
        // TODO: need to constrain this to unicode-only emojis somehow

        /// <summary>
        /// The unicode representation of this emote.
        /// </summary>
        public string Name { get; }

        public override string ToString() => Name;

        /// <summary>
        /// Creates a unicode emoji.
        /// </summary>
        /// <param name="unicode">The pure UTF-8 encoding of an emoji</param>
        public Emoji(string unicode)
        {
            // NETStandard1.1 doesn't support UTF32
#if !NETSTANDARD1_1
            byte[] utf32 = Encoding.UTF32.GetBytes(unicode);
            for (var i = 0; i < utf32.Length; i += 4)
            {
                int codepoint = BitConverter.ToInt32(utf32, i);
                bool any = false;

                for (var j = 0; j < Codepoints.Length; j++)
                {
                    if (Codepoints[j] == codepoint)
                    {
                        any = true;
                        break;
                    }
                }

                if (any) continue;
                else
                    throw new ArgumentException("One or more characters was not a valid Emoji", nameof(unicode));
            }
#endif

            Name = unicode;
        }

        public override bool Equals(object other)
        {
            if (other == null) return false;
            if (other == this) return true;

            var otherEmoji = other as Emoji;
            if (otherEmoji == null) return false;

            return string.Equals(Name, otherEmoji.Name);
        }

        public override int GetHashCode() => Name.GetHashCode();
    }
}
