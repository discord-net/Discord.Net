namespace Discord.API.Rest
{
    class GetAuditLogsParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<ulong> BeforeEntryId { get; set; }
        public Optional<ulong> UserId { get; set; }
        public Optional<int> ActionType { get; set; }
    }
}
