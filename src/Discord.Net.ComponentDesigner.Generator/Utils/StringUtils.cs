using System;

namespace Discord.CX;

public static class StringUtils
{
    public static string Prefix(this string str, int count, char prefixChar = ' ')
        => count > 0 ? $"{new string(prefixChar, count)}{str}" : str;

    public static string Postfix(this string str, int count, char prefixChar = ' ')
        => count > 0 ? $"{str}{new string(prefixChar, count)}" : str;

    public static string WithNewlinePadding(this string str, int pad)
        => str.Replace("\n", "\n".Postfix(pad));

    public static string WrapIfSome(this string str, string wrapping)
        => string.IsNullOrWhiteSpace(str) ? str : $"{wrapping}{str}{wrapping}";

    public static string PrefixIfSome(this string str, int count, char prefixChar = ' ')
        => string.IsNullOrWhiteSpace(str) ? str : $"{new string(prefixChar, count)}{str}";
    public static string PrefixIfSome(this string str, string prefix)
        => string.IsNullOrWhiteSpace(str) ? str : $"{prefix}{str}";

    public static string PostfixIfSome(this string str, int count, char prefixChar = ' ')
        => string.IsNullOrWhiteSpace(str) ? str : $"{str}{new string(prefixChar, count)}";
    public static string PostfixIfSome(this string str, string postfix)
        => string.IsNullOrWhiteSpace(str) ? str : $"{str}{postfix}";

    public static string Map(this string str, Func<string, string> mapper)
        => string.IsNullOrWhiteSpace(str) ? str : mapper(str);
}
