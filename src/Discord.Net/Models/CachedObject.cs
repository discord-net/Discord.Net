namespace Discord
{
    public abstract class CachedObject
	{
		protected readonly DiscordClient _client;
		private bool _isCached;

		internal CachedObject(DiscordClient client, string id)
		{
			_client = client;
			Id = id;
		}

		/// <summary> Returns the unique identifier for this object. </summary>
		public string Id { get; internal set; }

		public override string ToString() => $"{this.GetType().Name} {Id}";

		internal void Cache()
		{
			OnCached();
			_isCached = true;
		}
		internal void Uncache()
		{
			if (_isCached)
			{
				OnUncached();
				_isCached = false;
			}
		}
		internal abstract void OnCached();
		internal abstract void OnUncached();
	}
}
