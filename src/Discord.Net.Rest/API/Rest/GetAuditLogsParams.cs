namespace Discord.API.Rest
{
    class GetAuditLogsParams
    {
        public Optional<int> Limit { get; set; }
        public Optional<ulong> AfterEntryId { get; set; }
    }
}
