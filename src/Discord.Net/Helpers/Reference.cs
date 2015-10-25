using System;

namespace Discord
{
    internal class Reference<T>
		where T : CachedObject
    {
		private Action<T> _onCache, _onUncache;
		private Func<string, T> _getItem;
        private string _id;
		public string Id
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
				if (v != null && !_value.IsCached)
				{
					v = null;
					_value = null;
				}
				if (v == null && _id != null)
				{
					v = _getItem(_id);
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

		public Reference(Func<string, T> onUpdate, Action<T> onCache = null, Action<T> onUncache = null)
			: this(null, onUpdate, onCache, onUncache) { }
		public Reference(string id, Func<string, T> getItem, Action<T> onCache = null, Action<T> onUncache = null)
		{
			_id = id;
			_getItem = getItem;
			_onCache = onCache;
			_onUncache = onUncache;
		}
	}
}
