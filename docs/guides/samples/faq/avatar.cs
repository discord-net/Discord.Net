public async Task ChangeAvatar()
{
    var fileStream = new FileStream("./newAvatar.png", FileMode.Open);
    await (await _client.GetCurrentUserAsync()).ModifyAsync(x => x.Avatar = fileStream);
}