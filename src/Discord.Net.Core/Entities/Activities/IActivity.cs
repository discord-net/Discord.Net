namespace Discord
{
    public interface IActivity
    {
        string Name { get; }
        ActivityType Type { get; }
    }
}
