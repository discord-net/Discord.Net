namespace Discord.API
{
    internal struct EntityOrId<T>
    {
        public ulong Id { get; }
        public T Object { get; }

        public EntityOrId(ulong id)
        {
            Id = id;
            Object = default(T);
        }
        public EntityOrId(T obj)
        {
            Id = 0;
            Object = obj;
        }
    }
}
