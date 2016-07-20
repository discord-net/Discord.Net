using System;
using System.Collections.Generic;
using System.Reflection;

namespace Discord.Commands
{
    public class DependencyMap : IDependencyMap
    {
        private Dictionary<Type, object> map;

        public DependencyMap()
        {
            map = new Dictionary<Type, object>();
        }

        public object Get(Type t)
        {
            if (!map.ContainsKey(t))
                throw new KeyNotFoundException($"The dependency map does not contain \"{t.FullName}\"");
            return map[t];
        }

        public T Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        public void Add<T>(T obj)
        {
            var t = typeof(T);
            if (map.ContainsKey(t))
                throw new InvalidOperationException($"The dependency map already contains \"{t.FullName}\"");
            map.Add(t, obj);
        }
    }
}
