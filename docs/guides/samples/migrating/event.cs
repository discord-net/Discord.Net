_client.MessageReceived += async (msg) =>
{
    await msg.Channel.SendMessageAsync(msg.Content);
}