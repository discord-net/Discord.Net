namespace Discord
{
    public interface IOptional
    {
        object Value { get; }
        bool IsSpecified { get; }
    }
}
