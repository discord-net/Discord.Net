namespace Discord.API
{
    public struct ObjectOrId<T>
    {
        public ulong Id { get; }
        public T Object { get; }

        public ObjectOrId(ulong id)
        {
            Id = id;
            Object = default(T);
        }
        public ObjectOrId(T obj)
        {
            Id = 0;
            Object = obj;
        }
    }
}
