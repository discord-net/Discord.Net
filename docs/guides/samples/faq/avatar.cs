public async Task ChangeAvatar()
{
    var fileStream = new FileStream("./newAvatar.png", FileMode.Open);
    await _client.CurrentUser.ModifyAsync(x => x.Avatar = fileStream);
}