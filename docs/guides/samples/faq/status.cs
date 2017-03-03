public async Task ModifyStatus()
{
    await _client.SetStatusAsync(UserStatus.Idle);
    await _client.SetGameAsync("Type !help for help");
}
