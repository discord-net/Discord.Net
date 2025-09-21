using Discord.CX.Parser;
using Microsoft.CodeAnalysis.Text;

namespace Discord.CX.Parser;

partial class CXSourceText
{
    public sealed class StringSource(string text) : CXSourceText
    {
        public string Text { get; } = text;

        public override char this[int i] => Text[i];
        public override int Length => Text.Length;

        public override string this[int start, int length]
            => Text.Substring(start, length);
    }
}
