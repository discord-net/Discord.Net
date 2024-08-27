namespace Discord;

public enum ModifyApplicationFlags
{
    /// <summary>
    ///     Indicates that the app has enabled the GUILD_PRESENCES intent on a bot in less than 100 servers.
    /// </summary>
    GatewayPresenceLimited = 1 << 13,
    
    /// <summary>
    ///     Indicates that the app has enabled the GUILD_MEMBERS intent on a bot in less than 100 servers.
    /// </summary>
    GatewayGuildMembersLimited = 1 << 15,
    
    /// <summary>
    ///     Indicates that the app has enabled the MESSAGE_CONTENT intent on a bot in less than 100 servers.
    /// </summary>
    GatewayMessageContentLimited = 1 << 19,
}