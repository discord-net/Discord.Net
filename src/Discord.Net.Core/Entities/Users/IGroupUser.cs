namespace Discord
{
    /// <summary>
    ///     Represents a Discord user that is in a group.
    /// </summary>
    public interface IGroupUser : IUser, IVoiceState
    {
        ///// <summary> Kicks this user from this group. </summary>
        //Task KickAsync(RequestOptions options = null);
    }
}
