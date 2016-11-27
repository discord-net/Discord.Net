namespace Discord.API.Rest
{
    public class GetReactionUsersParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<ulong> AfterUserId { get; set; }
    }
}
