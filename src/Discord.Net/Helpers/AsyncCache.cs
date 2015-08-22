using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Discord.Helpers
{
    public class AsyncCache<TValue, TModel> : IEnumerable<TValue>
		where TValue : class
		where TModel : class
	{
		protected readonly ConcurrentDictionary<string, TValue> _dictionary;
		private readonly Func<string, string, TValue> _onCreate;
		private readonly Action<TValue, TModel> _onUpdate;
		private readonly Action<TValue> _onRemove;

		public AsyncCache(Func<string, string, TValue> onCreate, Action<TValue, TModel> onUpdate, Action<TValue> onRemove = null)
		{
			_dictionary = new ConcurrentDictionary<string, TValue>();
			_onCreate = onCreate;
			_onUpdate = onUpdate;
			_onRemove = onRemove;
        }

		public TValue this[string key]
		{
			get
			{
				if (key == null)
					return null;
				TValue value = null;
				_dictionary.TryGetValue(key, out value);
				return value;
			}
		}
		
		public TValue Update(string key, TModel model)
		{
			return Update(key, null, model);
		}
		public TValue Update(string key, string parentKey, TModel model)
		{
			if (key == null)
				return null;
			while (true) 
			{
				bool isNew;
				TValue value;
				isNew = !_dictionary.TryGetValue(key, out value);
                if (isNew)
					value = _onCreate(key, parentKey);
				if (model != null)
					_onUpdate(value, model);
				if (isNew)
				{
					//If this fails, repeat as an update instead of an add
					if (_dictionary.TryAdd(key, value))
						return value; 
				}
				else
				{
					_dictionary[key] = value;
					return value;
				}
            }
		}
		
        public TValue Remove(string key)
		{
			TValue value = null;
			if (_dictionary.TryRemove(key, out value))
			{
				if (_onRemove != null)
					_onRemove(value);
				return value;
			}
			else
				return null;
		}

		public void Clear()
		{
			_dictionary.Clear();
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dictionary.Values.GetEnumerator();
		}
	}
}
