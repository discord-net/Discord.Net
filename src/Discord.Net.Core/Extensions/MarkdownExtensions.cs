using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    internal static class MarkdownExtensions
    {
        public static string ToBold(this string text, int index = 0, int? count = null) //=> $"**{text}**";
        {
            var length = count ?? (text.Length - index);

            return text.Format($"**{text.Substring(index, (index + length))}**", index, length);
        }

        public static string ToItalic(this string text, int index = 0, int? count = null) //=> $"*{text}*";
        {
            var length = count ?? (text.Length - index);

            return text.Format($"*{text.Substring(index, (index + length))}*", index, length);
        }

        public static string ToUnderline(this string text, int index = 0, int? count = null) //=> $"__{text}__";
        {
            var length = count ?? (text.Length - index);

            return text.Format($"__{text.Substring(index, (index + length))}__", index, length);
        }

        public static string ToStrikethrough(this string text, int index = 0, int? count = null) //=> $"~~{text}~~";
        {
            var length = count ?? (text.Length - index);

            return text.Format($"~~{text.Substring(index, (index + length))}~~", index, length);
        }

        public static string ToSpoiler(this string text, int index = 0, int? count = null) //=> $"||{text}||";
        {
            var length = count ?? (text.Length - index);

            return text.Format($"||{text.Substring(index, (index + length))}||", index, length);
        }

        public static string ToQuote(this string text, int index = 0, int? count = null) //=> $"> {text}";
        {
            if (index is 0 && count is null)
                text = text.Replace(Environment.NewLine, $"{Environment.NewLine}> ");

            var length = count ?? (text.Length - index);

            return text.Format($"{Environment.NewLine}> {text.Substring(index, (index + length))}{Environment.NewLine}", index, length);
        }

        public static string ToBlockQuote(this string text, int index = 0, int? count = null) //=> $">>> {text}";
        {
            var length = count ?? (text.Length - index);

            return text.Format($"{Environment.NewLine}>>> {text.Substring(index, (index + length))}{Environment.NewLine}", index, length);
        }

        public static string ToCode(this string text, int index = 0, int? count = null) //=> $"`{text}`";
        {
            var length = count ?? (text.Length - index);

            return text.Format($"`{text.Substring(index, (index + length))}`", index, length);
        }

        public static string ToCodeBlock(this string text, CodeLanguage? lang = null, int index = 0, int? count = null)
        {
            lang ??= CodeLanguage.None;

            var length = count ?? (text.Length - index);

            return text.Format($"```{lang.Value}{Environment.NewLine}{text.Substring(index, (index + length))}{Environment.NewLine}```", index, length);
        }

        public static string ToHyperLink(this string text, string url, int index = 0, int? count = null)
        {
            var length = count ?? (text.Length - index);

            return text.Format($"[{text.Substring(index, (index + length))}]({url})", index, length);
        }

        public static string ToHeader(this string text, HeaderFormat format, int index = 0, int? count = null)
        {
            var length = count ?? (text.Length - index);

            return text.Format($"{Environment.NewLine}{format.Format} {text.Substring(index, (index + length))} {Environment.NewLine}", index, length);
        }

        public static string WithTimestamp(this string text, DateTime dateTime, TimestampTagStyles style, int index = 0)
            => text.Insert(index, TimestampTag.FromDateTime(dateTime, style).ToString());

        private static string Format(this string text, string format, int index, int length)
        {
            return text.Insert(index, format).Remove(index + format.Length, length);
        }
    }
}
