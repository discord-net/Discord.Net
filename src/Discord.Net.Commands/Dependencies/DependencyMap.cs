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

        /// <inheritdoc />
        public void Add<T>(T obj) where T : class 
            => AddFactory(() => obj);
        /// <inheritdoc />
        public bool TryAdd<T>(T obj) where T : class
            => TryAddFactory(() => obj);
        /// <inheritdoc />
        public void AddTransient<T>() where T : class, new() 
            => AddFactory(() => new T());
        /// <inheritdoc />
        public bool TryAddTransient<T>() where T : class, new()
            => TryAddFactory(() => new T());
        /// <inheritdoc />
        public void AddTransient<TKey, TImpl>() where TKey : class 
            where TImpl : class, TKey, new() 
            => AddFactory<TKey>(() => new TImpl());
        public bool TryAddTransient<TKey, TImpl>() where TKey : class
            where TImpl : class, TKey, new()
            => TryAddFactory<TKey>(() => new TImpl());
        
        /// <inheritdoc />
        public void AddFactory<T>(Func<T> factory) where T : class
        {
            var t = typeof(T);
            if (typeof(T) == typeof(IDependencyMap))
                throw new InvalidOperationException("IDependencyMap is used internally and cannot be added as a dependency");
            if (typeof(T) == typeof(CommandService))
                throw new InvalidOperationException("CommandService is used internally and cannot be added as a dependency");
            if (map.ContainsKey(t))
                throw new InvalidOperationException($"The dependency map already contains \"{t.FullName}\"");
            map.Add(t, factory);
        }
        /// <inheritdoc />
        public bool TryAddFactory<T>(Func<T> factory) where T : class
        {
            var t = typeof(T);
            if (map.ContainsKey(t))
                return false;
            if (typeof(T) == typeof(IDependencyMap))
                throw new InvalidOperationException("IDependencyMap is used internally and cannot be added as a dependency");
            if (typeof(T) == typeof(CommandService))
                throw new InvalidOperationException("CommandService is used internally and cannot be added as a dependency");
            map.Add(t, factory);
            return true;
        }

        /// <inheritdoc />
        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }
        /// <inheritdoc />
        public object Get(Type t)
        {
            object result;
            if (!TryGet(t, out result))
                throw new KeyNotFoundException($"The dependency map does not contain \"{t.FullName}\"");
            else
                return result;
        }

        /// <inheritdoc />
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
        /// <inheritdoc />
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
