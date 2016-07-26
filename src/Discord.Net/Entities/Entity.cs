namespace Discord
{
    internal abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; }

        public abstract DiscordClient Discord { get; }

        internal virtual bool IsAttached => false;
        bool IEntity<T>.IsAttached => IsAttached;

        public Entity(T id)
        {
            Id = id;
        }
    }
}
