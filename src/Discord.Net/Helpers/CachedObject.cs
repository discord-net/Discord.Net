using System.Globalization;

namespace Discord
{
	public abstract class CachedObject<TKey> : CachedObject
	{
		private TKey _id;

		internal CachedObject(DiscordClient client, TKey id)
			: base(client)
		{
			_id = id;
		}

		/// <summary> Returns the unique identifier for this object. </summary>
		public TKey Id { get { return _id; } internal set { _id = value; } }

		public override string ToString() => $"{this.GetType().Name} {Id}";
	}

	public abstract class CachedObject
	{
		protected readonly DiscordClient _client;
		private bool _isCached;

		internal DiscordClient Client => _client;
		internal bool IsCached => _isCached;

        internal CachedObject(DiscordClient client)
		{
			_client = client;
		}

		internal void Cache()
		{
			LoadReferences();
			_isCached = true;
		}
		internal void Uncache()
		{
			if (_isCached)
			{
				UnloadReferences();
				_isCached = false;
			}
		}
		internal abstract void LoadReferences();
		internal abstract void UnloadReferences();
	}
}
