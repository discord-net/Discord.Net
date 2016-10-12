namespace Discord
{
    internal class PageInfo
    {
        public int Page { get; set; }
        public ulong? Position { get; set; }
        public int? Count { get; set; }
        public int PageSize { get; set; }
        public int? Remaining { get; set; }

        internal PageInfo(ulong? pos, int? count, int pageSize)
        {
            Page = 1;
            Position = pos;
            Count = count;
            Remaining = count;
            PageSize = pageSize;

            if (Count != null && Count.Value < PageSize)
                PageSize = Count.Value;
        }
    }
}
