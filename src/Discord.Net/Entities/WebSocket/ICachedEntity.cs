namespace Discord
{
    internal interface ICachedEntity<T> : IEntity<T>
    {
        DiscordSocketClient Discord { get; }
    }
}
