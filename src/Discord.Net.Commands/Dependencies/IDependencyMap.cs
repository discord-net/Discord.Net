using System;

namespace Discord.Commands
{
    public interface IDependencyMap
    {
        void Add<T>(T obj);

        T Get<T>();
        bool TryGet<T>(out T result);

        object Get(Type t);
        bool TryGet(Type t, out object result);
    }
}
