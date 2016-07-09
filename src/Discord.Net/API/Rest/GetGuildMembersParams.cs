namespace Discord.API.Rest
{
    public class GetGuildMembersParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<ulong> AfterUserId { get; set; }
    }
}
