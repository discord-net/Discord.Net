namespace Discord
{
    /// <summary> Represents a Discord user's presence status. </summary>
    public interface IPresence
    {
        /// <summary> Gets the activity this user is currently doing. </summary>
        IActivity Activity { get; }
        /// <summary> Gets the current status of this user. </summary>
        UserStatus Status { get; }
    }
}
