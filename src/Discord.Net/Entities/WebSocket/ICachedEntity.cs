namespace Discord
{
    interface ICachedEntity<T> : IEntity<T>
    {
        DiscordSocketClient Discord { get; }
    }
}
