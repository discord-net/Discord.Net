client.CreateCommandGroup("invites", invites =>
{
	invites.DefaultMinPermissions((int)Permissions.Admin);
	
	//Usage: invites accept [inviteCode]
	invites.CreateCommand("accept")
		.ArgsEqual(1)
		.Do(async e =>
		{
			try
			{
				await _client.AcceptInvite(e.Args[0]);
				await _client.SendMessage(e.Channel, "Invite \"" + e.Args[0] + "\" accepted.");
			}
			catch (HttpException ex)
			{
				await _client.SendMessage(e.Channel, "Error: " + ex.Message);
			}
		});
});