public string GetChannelTopic(ulong id)
{
	var channel = client.GetChannel(81384956881809408) as SocketTextChannel;
	return channel?.Topic;
}

public SocketGuildUser GetGuildOwner(SocketChannel channel)
{
	var guild = (channel as SocketGuildChannel)?.Guild;
	return guild?.Owner;
}