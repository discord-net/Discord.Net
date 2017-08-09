using System;
using System.Collections.Concurrent;

namespace Discord.Serialization
{
    public interface ISelectorGroup
    {
        Type Type { get; }

        void AddDynamicConverter(object key, object converter);
        object GetDynamicConverter(Delegate getKeyFunc, object model);
    }

    internal class SelectorGroup<TKey> : ISelectorGroup
    {
        private ConcurrentDictionary<TKey, object> _mapping;

        public Type Type => typeof(TKey);

        public SelectorGroup()
        {
            _mapping = new ConcurrentDictionary<TKey, object>();
        }

        public void AddDynamicConverter(object key, object converter)
            => _mapping[(TKey)key] = converter;

        public object GetDynamicConverter(Delegate getKeyFunc, object model)
        {
            var keyFunc = getKeyFunc as Func<object, TKey>;
            var key = keyFunc(model);
            if (key == null)
                return null;
            if (_mapping.TryGetValue(key, out var converter))
                return converter;
            
            return null;
        }
    }
}
