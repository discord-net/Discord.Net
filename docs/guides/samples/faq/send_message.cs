public async Task SendMessageToChannel(ulong ChannelId)
{
    var channel = await _client.GetChannelAsync(ChannelId) as IMessageChannel;
    await channel?.SendMessageAsync("aaaaaaaaahhh!!!")
    /*           ^ This question mark is used to indicate that 'channel' may sometimes be null, and in cases that it is null, we will do nothing here. */ 
}