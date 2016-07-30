public async Task ModifyStatus()
{
    await (await _client.GetCurrentUserAsync()).ModifyStatusAsync(x =>
    {
        x.Status = UserStatus.Idle;
        x.Game = new Game("Type !help for help");
    });
}