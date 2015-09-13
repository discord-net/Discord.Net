using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Collections
{
	public abstract class AsyncCollection<TValue> : IEnumerable<TValue>
		where TValue : class
	{
		private static readonly object _writerLock = new object();

		internal class CollectionItemEventArgs : EventArgs
		{
			public TValue Item { get; }
			public CollectionItemEventArgs(TValue item) { Item = item; }
		}

		internal EventHandler<CollectionItemEventArgs> ItemCreated;
		private void RaiseItemCreated(TValue item)
		{
			if (ItemCreated != null)
				ItemCreated(this, new CollectionItemEventArgs(item));
		}
		internal EventHandler<CollectionItemEventArgs> ItemUpdated;
		protected void RaiseItemUpdated(TValue item)
		{
			if (ItemUpdated != null)
				ItemUpdated(this, new CollectionItemEventArgs(item));
		}
		internal EventHandler<CollectionItemEventArgs> ItemDestroyed;
		private void RaiseItemDestroyed(TValue item)
		{
			if (ItemDestroyed != null)
				ItemDestroyed(this, new CollectionItemEventArgs(item));
		}

		protected readonly DiscordClient _client;
		protected readonly ConcurrentDictionary<string, TValue> _dictionary;

		protected AsyncCollection(DiscordClient client)
		{
			_client = client;
			_dictionary = new ConcurrentDictionary<string, TValue>();
		}

		protected TValue Get(string key)
		{
			TValue result;
			if (!_dictionary.TryGetValue(key, out result))
				return null;
			return result;
		}
		protected TValue GetOrAdd(string key, Func<TValue> createFunc)
		{
			TValue result;
			if (_dictionary.TryGetValue(key, out result))
				return result;

			lock (_writerLock)
			{
				TValue newItem = createFunc();
				result = _dictionary.GetOrAdd(key, newItem);
				if (result == newItem)
				{
					OnCreated(newItem);
					RaiseItemCreated(result);
				}
			}
			return result;
		}
		protected TValue TryRemove(string key)
		{
			if (_dictionary.ContainsKey(key))
			{
				lock (_writerLock)
				{
					TValue result;
					if (_dictionary.TryRemove(key, out result))
						return result;
				}
			}
			return null;
		}
		protected TValue Remap(string oldKey, string newKey)
		{
			if (_dictionary.ContainsKey(oldKey))
			{
				lock (_writerLock)
				{
					TValue result;
					if (_dictionary.TryRemove(oldKey, out result))
						_dictionary[newKey] = result;
					return result;
				}
			}
			return null;
		}
		protected internal void Clear()
		{
			lock (_writerLock)
				_dictionary.Clear();
		}

		protected abstract void OnCreated(TValue item);
		protected abstract void OnRemoved(TValue item);

		public IEnumerator<TValue> GetEnumerator()
		{
			return _dictionary.Select(x => x.Value).GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
