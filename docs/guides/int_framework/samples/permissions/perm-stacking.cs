[DefaultMemberPermissions(GuildPermission.SendMessages | GuildPermission.ViewChannels)]
[SlashCommand("ping", "Pong!")]
public async Task Ping()
    => await RespondAsync("pong");
