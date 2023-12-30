namespace Discord;

public readonly struct RatelimitInfo(uint limit, uint remaining, DateTimeOffset reset)
{
    public readonly uint Limit = limit;
    public readonly uint Remaining = remaining;
    public readonly DateTimeOffset Reset = reset;
}
