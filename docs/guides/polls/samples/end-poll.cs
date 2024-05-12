// Get a message with a poll
var message = await channel.GetMessageAsync(id) as IUserMessage;
// End the poll
await message.EndPollAsync();