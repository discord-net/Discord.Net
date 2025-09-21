namespace Discord.CX.Parser;

internal static class TextUtils
{
    public static bool IsNewline(this char ch)
    {
        return ch is '\r' or '\n' or '\u0085' or '\u2028' or '\u2029';
    }
}
