namespace Discord.API.Rest
{
    internal class SearchGuildMembersParams
    {
        public string Query { get; set; }
        public Optional<int> Limit { get; set; }
    }
}
