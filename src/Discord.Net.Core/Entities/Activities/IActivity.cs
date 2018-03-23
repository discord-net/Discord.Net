namespace Discord
{
    /// <summary> A Discord activity. </summary>
    public interface IActivity
    {
        /// <summary> Gets the name of the activity. </summary>
        string Name { get; }
        /// <summary> Gets the type of the activity. </summary>
        ActivityType Type { get; }
    }
}
