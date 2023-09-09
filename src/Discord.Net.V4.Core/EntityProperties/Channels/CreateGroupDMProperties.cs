namespace Discord;

public sealed class CreateGroupDMProperties
{
    public required string[] AccessTokens { get; set; }
    public required Dictionary<ulong, string> Nicknames { get; set; }
}
