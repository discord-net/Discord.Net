public string GetChannelTopic(ulong id)
{
	var channel = client.GetChannel(81384956881809408) as SocketTextChannel;
	if (channel == null) return "";
	return channel.Topic;
}

public string GuildOwner(SocketChannel channel)
{
	var guild = (channel as SocketGuildChannel)?.Guild;
	if (guild == null) return "";
	return Context.Guild.Owner.Username;
}