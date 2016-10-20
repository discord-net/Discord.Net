using System;
using System.Collections.Generic;

namespace Discord.Commands
{
    public class DependencyMap : IDependencyMap
    {
        private Dictionary<Type, object> map;

        public static DependencyMap Empty => new DependencyMap();

        public DependencyMap()
        {
            map = new Dictionary<Type, object>();
        }

        public void Add<T>(T obj)
        {
            var t = typeof(T);
            if (map.ContainsKey(t))
                throw new InvalidOperationException($"The dependency map already contains \"{t.FullName}\"");
            map.Add(t, obj);
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
            return map.TryGetValue(t, out result);
        }
    }
}
