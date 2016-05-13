namespace Discord.API
{
    public interface IOptional
    {
        object Value { get; }
        bool IsSpecified { get; }
    }
}
