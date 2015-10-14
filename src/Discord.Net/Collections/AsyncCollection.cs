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
		private readonly object _writerLock;

		internal class CollectionItemEventArgs : EventArgs
		{
			public TValue Item { get; }
			public CollectionItemEventArgs(TValue item) { Item = item; }
		}
		internal class CollectionItemRemappedEventArgs : EventArgs
		{
			public TValue Item { get; }
			public string OldId { get; }
			public string NewId { get; }
			public CollectionItemRemappedEventArgs(TValue item, string oldId, string newId) { Item = item; OldId = oldId; NewId = newId; }
		}

		internal EventHandler<CollectionItemEventArgs> ItemCreated;
		private void RaiseItemCreated(TValue item)
		{
			if (ItemCreated != null)
				ItemCreated(this, new CollectionItemEventArgs(item));
		}
		internal EventHandler<CollectionItemEventArgs> ItemDestroyed;
		private void RaiseItemDestroyed(TValue item)
		{
			if (ItemDestroyed != null)
				ItemDestroyed(this, new CollectionItemEventArgs(item));
		}
		internal EventHandler<CollectionItemRemappedEventArgs> ItemRemapped;
		private void RaiseItemRemapped(TValue item, string oldId, string newId)
		{
			if (ItemRemapped != null)
				ItemRemapped(this, new CollectionItemRemappedEventArgs(item, oldId, newId));
		}

		internal EventHandler Cleared;
		private void RaiseCleared()
		{
			if (Cleared != null)
				Cleared(this, EventArgs.Empty);
		}

		protected readonly DiscordClient _client;
		protected readonly ConcurrentDictionary<string, TValue> _dictionary;

		protected AsyncCollection(DiscordClient client, object writerLock)
		{
			_client = client;
			_writerLock = writerLock;
			_dictionary = new ConcurrentDictionary<string, TValue>();
		}

		protected TValue Get(string key)
		{
			if (key == null)
				return null;

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
					{
						OnRemoved(result);
						return result;
					}
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
			{
				_dictionary.Clear();
				RaiseCleared();
			}
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
