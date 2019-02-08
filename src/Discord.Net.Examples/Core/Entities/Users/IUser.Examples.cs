#region GetAvatarUrl
public async Task GetAvatarAsync(IUser user)
{
    var userAvatarUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
    await textChannel.SendMessageAsync(userAvatarUrl);
}
#endregion

#region GetOrCreateDMChannelAsync
public async Task MessageUserAsync(IUser user)
{
    var channel = await user.GetOrCreateDMChannelAsync();
    try
    {
        await channel.SendMessageAsync("Awesome stuff!");
    }
    catch (Discord.Net.HttpException ex) when (ex.HttpCode == 403)
    {
        Console.WriteLine($"Boo, I cannot message {user}");
    }
}
#endregion