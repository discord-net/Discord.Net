using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Discord
{
	internal abstract class AsyncCollection<TKey, TValue> : IEnumerable<TValue>
		where TKey : struct, IEquatable<TKey>
		where TValue : CachedObject
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
			public TKey OldId { get; }
			public TKey NewId { get; }
			public CollectionItemRemappedEventArgs(TValue item, TKey oldId, TKey newId) { Item = item; OldId = oldId; NewId = newId; }
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
		private void RaiseItemRemapped(TValue item, TKey oldId, TKey newId)
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
		protected readonly ConcurrentDictionary<TKey, TValue> _dictionary;

		public int Count => _dictionary.Count;

		protected AsyncCollection(DiscordClient client, object writerLock)
		{
			_client = client;
			_writerLock = writerLock;
			_dictionary = new ConcurrentDictionary<TKey, TValue>();
        }

		public TValue this[TKey? key]
			=> key == null ? null : this[key.Value];
        public TValue this[TKey key]
		{
			get
			{
				if (key.Equals(default(TKey)))
					return null;

				TValue result;
				if (!_dictionary.TryGetValue(key, out result))
					return null;
				return result;
			}
		}
		protected TValue GetOrAdd(TKey key, Func<TValue> createFunc)
		{
			TValue result;
			if (_dictionary.TryGetValue(key, out result))
				return result;

			lock (_writerLock)
			{
				if (!_dictionary.ContainsKey(key))
				{
					result = createFunc();
					if (result.Cache())
					{
						_dictionary.TryAdd(key, result);
						RaiseItemCreated(result);
					}
					else
						result.Uncache();
					return result;
				}
				else
					return _dictionary[key];
			}
		}
		protected void Import(IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			lock (_writerLock)
			{
				foreach (var pair in items)
				{
					var value = pair.Value;
					if (value.Cache())
					{
						_dictionary.TryAdd(pair.Key, value);
						RaiseItemCreated(value);
					}
					else
						value.Uncache();
				}
			}
		}

		public TValue TryRemove(TKey key)
		{
			if (_dictionary.ContainsKey(key))
			{
				lock (_writerLock)
				{
					TValue result;
					if (_dictionary.TryRemove(key, out result))
					{
						result.Uncache(); //TODO: If this object is accessed before OnRemoved finished firing, properties such as Server.Channels will have null elements
						return result;
					}
				}
			}
			return null;
		}
		public void Clear()
		{
			lock (_writerLock)
			{
				_dictionary.Clear();
                RaiseCleared();
			}
        }

		public TValue Remap(TKey oldKey, TKey newKey)
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

		public IEnumerator<TValue> GetEnumerator() => _dictionary.Select(x => x.Value).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
