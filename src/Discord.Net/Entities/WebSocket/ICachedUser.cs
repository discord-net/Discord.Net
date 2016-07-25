namespace Discord
{
    internal interface ICachedUser : IUser, ICachedEntity<ulong>
    {
        CachedGlobalUser User { get; }

        ICachedUser Clone();
    }
}
