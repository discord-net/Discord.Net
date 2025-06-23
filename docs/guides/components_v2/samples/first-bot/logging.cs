private static Task Log(LogMessage msg)
{
	Console.WriteLine(msg.ToString());
	return Task.CompletedTask;
}