// Bad code!!!
var result = await _commands.ExecuteAsync(context, argPos, _services);
if (result.CommandError != null)
	switch(result.CommandError)
	{
		case CommandError.BadArgCount:
			await context.Channel.SendMessageAsync(
				"Parameter count does not match any command's.");
			break;
		default:
			await context.Channel.SendMessageAsync(
				$"An error has occurred {result.ErrorReason}");
			break;
	}