[RequireOwner]
[Command("echo")]
public Task EchoAsync(string input) => ReplyAsync(input);