namespace Discord.Rest
{
    public struct MemberRoleEditInfo
    {
        internal MemberRoleEditInfo(string name, ulong roleId, bool added)
        {
            Name = name;
            RoleId = roleId;
            Added = added;
        }

        public string Name { get; }
        public ulong RoleId { get; }
        public bool Added { get; }
    }
}
