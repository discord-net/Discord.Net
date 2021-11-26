[Command("process", RunMode = RunMode.Async)]
public async Task ProcessAsync(string input)
{
	// Does heavy calculation here.
	await Task.Delay(TimeSpan.FromMinute(1));
	await ReplyAsync(input);
}