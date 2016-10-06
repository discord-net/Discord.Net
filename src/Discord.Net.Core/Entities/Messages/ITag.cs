namespace Discord
{
    public interface ITag
    {
        int Index { get; }
        int Length { get; }
        TagType Type { get; }
        ulong Key { get; }
        object Value { get; }
    }
}
