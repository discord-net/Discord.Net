namespace Discord
{
    public struct PermissionOverwrite
    {
        public PermissionTarget TargetType { get; }
        public ulong TargetId { get; }
        public TriStateChannelPermissions Permissions { get; }
    }
}
