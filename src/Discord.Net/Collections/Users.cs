namespace Discord.Collections
{
	internal sealed class Users : AsyncCollection<User>
	{
		public Users(DiscordClient client, object writerLock)
			: base(client, writerLock) { }

		public User GetOrAdd(string id) => GetOrAdd(id, () => new User(_client, id));
	}
}
