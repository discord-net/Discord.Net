namespace Discord;

public static class AttachmentUtils
{
    public const string SpoilerPrefix = "SPOILER_";

    public static bool IsSpoiler(string name)
        => name.StartsWith(SpoilerPrefix);

    public static string AppendSpoilerPrefix(string str)
        => $"{SpoilerPrefix}{str}";
}
