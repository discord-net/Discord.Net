[RequireOwner]
[SlashCommand("hi")]
public Task SayHiAsync() => RespondAsync("hello owner!");
