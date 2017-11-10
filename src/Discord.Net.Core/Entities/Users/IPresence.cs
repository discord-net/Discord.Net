namespace Discord
{
    public interface IPresence
    {
        /// <summary> Gets the activity this user is currently doing. </summary>
        IActivity Activity { get; }
        /// <summary> Gets the current status of this user. </summary>
        UserStatus Status { get; }
    }
}