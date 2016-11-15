public async Task ModifyStatus()
{
    await _client.SetStatus(UserStatus.Idle);
    await _client.SetGame("Type !help for help");
}
