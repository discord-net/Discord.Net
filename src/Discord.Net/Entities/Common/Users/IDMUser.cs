namespace Discord
{
    public interface IDMUser : IUser
    {
        /// <summary> Gets the private channel with this user. </summary>
        IDMChannel Channel { get; }
    }
}