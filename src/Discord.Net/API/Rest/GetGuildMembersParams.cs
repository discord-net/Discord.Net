namespace Discord.API.Rest
{
    public class GetGuildMembersParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<int> Offset { get; set; }
    }
}
