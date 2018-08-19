namespace Discord.API.Rest
{
    internal class GetAuditLogsParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<ulong> BeforeEntryId { get; set; }
    }
}
