using System;

namespace Discord.Commands
{
    public interface IDependencyMap
    {
        void Add<T>(T obj) where T : class;
        void AddTransient<T>() where T : class, new();
        void AddTransient<TKey, TImpl>() where TKey: class where TImpl : class, TKey, new();
        void AddFactory<T>(Func<T> factory) where T : class;

        T Get<T>();
        bool TryGet<T>(out T result);

        object Get(Type t);
        bool TryGet(Type t, out object result);
    }
}
