using Discord;
using Discord.WebSocket;

public class LoggingService
{
	public LoggingService(DiscordSocketClient client, CommandService command)
	{
		client.Log += LogAsync;
		command.Log += LogAsync;
	}
	private Task LogAsync(LogMessage message)
	{
		if (message.Exception is CommandException cmdException)
		{
			Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
				+ $" failed to execute in {cmdException.Context.Channel}.");
			Console.WriteLine(cmdException);
		}
		else 
			Console.WriteLine($"[General/{message.Severity}] {message}");

		return Task.CompletedTask;
	}
}