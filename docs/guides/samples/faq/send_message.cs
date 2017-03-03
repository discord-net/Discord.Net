public async Task SendMessageToChannel(ulong ChannelId)
{
    var channel = _client.GetChannel(ChannelId) as SocketMessageChannel;
    await channel?.SendMessageAsync("aaaaaaaaahhh!!!")
    /*           ^ This question mark is used to indicate that 'channel' may sometimes be null, and in cases that it is null, we will do nothing here. */ 
}