namespace Discord
{
    internal abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; }

        public abstract DiscordClient Discord { get; }

        public bool IsAttached => this is ICachedEntity<T>;

        public Entity(T id)
        {
            Id = id;
        }
    }
}
