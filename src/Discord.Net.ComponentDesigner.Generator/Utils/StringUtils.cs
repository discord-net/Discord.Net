namespace Discord.ComponentDesigner.Generator;

public static class StringUtils
{
    public static string Prefix(this string str, int count, char prefixChar = ' ')
        => count > 0 ? $"{new string(prefixChar, count)}{str}" : str;

    public static string Postfix(this string str, int count, char prefixChar = ' ')
        => count > 0 ? $"{str}{new string(prefixChar, count)}" : str;

    public static string WithNewlinePadding(this string str, int pad)
        => str.Replace("\n", "\n".Postfix(pad));
}
