namespace Discord
{
    public class ModifyGuildRoleParams
    {
        public Optional<string> Name { get; set; }
        public Optional<ulong> Permissions { get; set; }
        public Optional<int> Position { get; set; }
        public Optional<uint> Color { get; set; }
        public Optional<bool> Hoist { get; set; }
    }
}
