public async Task MessageReceived(IMessage msg)
{
    var guild = (msg.Channel as IGuildChannel)?.Guild;
}