namespace Discord
{
    public class ModifyGuildMemberParams
    {
        public Optional<bool> Mute { get; set; }
        public Optional<bool> Deaf { get; set; }
        public Optional<string> Nickname { get; set; }
        public Optional<ulong[]> RoleIds { get; set; }
        public Optional<ulong> ChannelId { get; set; }
    }
}
