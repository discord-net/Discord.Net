namespace Discord
{
    internal class PageInfo
    {
        public int Page { get; set; }
        public ulong? Position { get; set; }
        public uint? Count { get; set; }
        public int PageSize { get; set; }
        public uint? Remaining { get; set; }

        internal PageInfo(ulong? pos, uint? count, int pageSize)
        {
            Page = 1;
            Position = pos;
            Count = count;
            Remaining = count;
            PageSize = pageSize;
        }
    }
}
