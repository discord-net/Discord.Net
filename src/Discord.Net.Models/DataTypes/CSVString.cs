namespace Discord.Models;

public readonly record struct CSVString<T>(
    ICollection<T> Items
)
{
    public override string ToString()
        => string.Join(",", Items);
}