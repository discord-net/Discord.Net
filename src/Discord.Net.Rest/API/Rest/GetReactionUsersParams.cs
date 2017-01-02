namespace Discord.API.Rest
{
    internal class GetReactionUsersParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<ulong> AfterUserId { get; set; }
    }
}
