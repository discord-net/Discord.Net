namespace Discord
{
    public struct PermissionOverwriteEntry
    {
        public PermissionTarget TargetType { get; }
        public ulong TargetId { get; }
        public OverwritePermissions Permissions { get; }
    }
}
