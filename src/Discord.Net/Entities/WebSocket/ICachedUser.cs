namespace Discord
{
    internal interface ICachedUser : IUser, ICachedEntity<ulong>
    {
        ICachedUser Clone();
    }
}
