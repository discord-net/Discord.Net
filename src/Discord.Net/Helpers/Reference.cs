using System;

namespace Discord
{
    internal struct Reference<T>
		where T : CachedObject<long>
	{
		private Action<T> _onCache, _onUncache;
		private Func<long, T> _getItem;
        private long? _id;
		public long? Id
		{
			get { return _id; }
			set
			{
				_id = value;
				_value = null;
			}
		}

		private T _value;
		public T Value
		{
			get
			{
				var v = _value; //A little trickery to make this threadsafe
				var id = _id;
				if (v != null && !_value.IsCached)
				{
					v = null;
					_value = null;
				}
				if (v == null && id != null)
				{
					v = _getItem(id.Value);
					if (v != null && _onCache != null)
						_onCache(v);
					_value = v;
				}
				return v;
			}
		}

		public T Load()
		{
			return Value; //Used for precaching
		}

		public void Unload()
		{
			if (_onUncache != null)
			{
				var v = _value;
				if (v != null && _onUncache != null)
					_onUncache(v);
			}
		}

		public Reference(Func<long, T> onUpdate, Action<T> onCache = null, Action<T> onUncache = null)
			: this(null, onUpdate, onCache, onUncache) { }
		public Reference(long? id, Func<long, T> getItem, Action<T> onCache = null, Action<T> onUncache = null)
		{
			_id = id;
			_getItem = getItem;
			_onCache = onCache;
			_onUncache = onUncache;
			_value = null;
        }
	}
}
