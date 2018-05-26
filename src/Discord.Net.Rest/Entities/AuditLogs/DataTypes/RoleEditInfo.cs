namespace Discord.Rest
{
    public struct RoleEditInfo
    {
        internal RoleEditInfo(Color? color, bool? mentionable, bool? hoist, string name,
            GuildPermissions? permissions)
        {
            Color = color;
            Mentionable = mentionable;
            Hoist = hoist;
            Name = name;
            Permissions = permissions;
        }

        public Color? Color { get; }
        public bool? Mentionable { get; }
        public bool? Hoist { get; }
        public string Name { get; }
        public GuildPermissions? Permissions { get; }
    }
}
