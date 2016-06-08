namespace Discord
{
    public interface IPresence
    {
        /// <summary> Gets the game this user is currently playing, if any. </summary>
        Game? Game { get; }
        /// <summary> Gets the current status of this user. </summary>
        UserStatus Status { get; }
    }
}