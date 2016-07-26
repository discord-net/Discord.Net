namespace Discord
{
    internal abstract class Entity<T> : IEntity<T>
    {
        public T Id { get; }

        public abstract DiscordClient Discord { get; }

        bool IEntity<T>.IsAttached => false;

        public Entity(T id)
        {
            Id = id;
        }
    }
}
