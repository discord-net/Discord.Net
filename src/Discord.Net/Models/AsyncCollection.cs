using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	internal abstract class AsyncCollection<TValue> : IEnumerable<TValue>
		where TValue : class
	{
		private readonly object _writerLock;

		public class CollectionItemEventArgs : EventArgs
		{
			public TValue Item { get; }
			public CollectionItemEventArgs(TValue item) { Item = item; }
		}
		public class CollectionItemRemappedEventArgs : EventArgs
		{
			public TValue Item { get; }
			public string OldId { get; }
			public string NewId { get; }
			public CollectionItemRemappedEventArgs(TValue item, string oldId, string newId) { Item = item; OldId = oldId; NewId = newId; }
		}

		public EventHandler<CollectionItemEventArgs> ItemCreated;
		private void RaiseItemCreated(TValue item)
		{
			if (ItemCreated != null)
				ItemCreated(this, new CollectionItemEventArgs(item));
		}
		public EventHandler<CollectionItemEventArgs> ItemDestroyed;
		private void RaiseItemDestroyed(TValue item)
		{
			if (ItemDestroyed != null)
				ItemDestroyed(this, new CollectionItemEventArgs(item));
		}
		public EventHandler<CollectionItemRemappedEventArgs> ItemRemapped;
		private void RaiseItemRemapped(TValue item, string oldId, string newId)
		{
			if (ItemRemapped != null)
				ItemRemapped(this, new CollectionItemRemappedEventArgs(item, oldId, newId));
		}

		public EventHandler Cleared;
		private void RaiseCleared()
		{
			if (Cleared != null)
				Cleared(this, EventArgs.Empty);
		}

		protected readonly DiscordClient _client;
		protected readonly ConcurrentDictionary<string, TValue> _dictionary;
		private readonly Action<TValue> _onCache, _onUncache;

		protected AsyncCollection(DiscordClient client, object writerLock, Action<TValue> onCache, Action<TValue> onUncache)
		{
			_client = client;
			_writerLock = writerLock;
			_dictionary = new ConcurrentDictionary<string, TValue>();
			_onCache = onCache;
			_onUncache = onUncache;
        }

		protected virtual void Initialize() { }

		public TValue this[string key]
		{
			get
			{
				if (key == null)
					return null;

				TValue result;
				if (!_dictionary.TryGetValue(key, out result))
					return null;
				return result;
			}
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
					_onCache(result);
					RaiseItemCreated(result);
				}
			}
			return result;
		}
		public TValue TryRemove(string key)
		{
			if (_dictionary.ContainsKey(key))
			{
				lock (_writerLock)
				{
					TValue result;
					if (_dictionary.TryRemove(key, out result))
					{
						_onUncache(result); //TODO: If this object is accessed before OnRemoved finished firing, properties such as Server.Channels will have null elements
						return result;
					}
				}
			}
			return null;
		}
		public TValue Remap(string oldKey, string newKey)
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
		public void Clear()
		{
			lock (_writerLock)
			{
				_dictionary.Clear();
				Initialize();
                RaiseCleared();
			}
        }

		public IEnumerator<TValue> GetEnumerator() => _dictionary.Select(x => x.Value).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
