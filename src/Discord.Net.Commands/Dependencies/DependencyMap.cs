using System;
using System.Collections.Generic;

namespace Discord.Commands
{
    public class DependencyMap : IDependencyMap
    {
        private Dictionary<Type, Func<object>> map;

        public static DependencyMap Empty => new DependencyMap();

        public DependencyMap()
        {
            map = new Dictionary<Type, Func<object>>();
        }

        public void Add<T>(T obj) where T : class 
            => AddFactory(() => obj);
        public void AddTransient<T>() where T : class, new() 
            => AddFactory(() => new T());
        public void AddTransient<TKey, TImpl>() where TKey : class 
            where TImpl : class, TKey, new() 
            => AddFactory<TKey>(() => new TImpl());

        public void AddFactory<T>(Func<T> factory) where T : class
        {
            var t = typeof(T);
            if (map.ContainsKey(t))
                throw new InvalidOperationException($"The dependency map already contains \"{t.FullName}\"");
            map.Add(t, factory);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }
        public object Get(Type t)
        {
            object result;
            if (!TryGet(t, out result))
                throw new KeyNotFoundException($"The dependency map does not contain \"{t.FullName}\"");
            else
                return result;
        }

        public bool TryGet<T>(out T result)
        {
            object untypedResult;
            if (TryGet(typeof(T), out untypedResult))
            {
                result = (T)untypedResult;
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }
        public bool TryGet(Type t, out object result)
        {
            Func<object> func;
            if (map.TryGetValue(t, out func))
            {
                result = func();
                return true;
            }
            result = null;
            return false;
        }
    }
}
