namespace Discord.API.Rest
{
    public class GetGuildMembersParams
    {
        public int? Limit { get; set; } = null;
        public int Offset { get; set; } = 0;
    }
}
