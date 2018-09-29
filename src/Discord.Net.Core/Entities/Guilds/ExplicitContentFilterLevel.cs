namespace Discord
{
    public enum ExplicitContentFilterLevel
    {
        /// <summary> No messages will be scanned. </summary>
        Disabled = 0,
        /// <summary> Scans messages from all guild members that do not have a role. </summary>
        /// <remarks> Recommented option for servers that use roles for trusted membership. </remarks>
        MembersWithoutRoles = 1,
        /// <summary> Scan messages sent by all guild members. </summary>
        AllMembers = 2
    }
}
