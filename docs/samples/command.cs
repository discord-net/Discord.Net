public enum Permissions
{
	User,
	Moderator,
	Admin
}

//Usage: say [text]
client.CreateCommand("say")
	.ArgsEqual(1)
	.MinPermissions((int)Permissions.User)
	.Do(async e =>
	{
		string msg = Format.Normal(e.CommandText);
		await _client.SendMessage(e.Channel, msg);
	});