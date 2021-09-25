namespace Discord.API.Rest
{
    internal class GetGuildSummariesParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<ulong> AfterGuildId { get; set; }
    }
}
