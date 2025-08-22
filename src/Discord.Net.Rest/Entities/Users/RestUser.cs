using Discord.Models;
using Discord.Models.Models;

namespace Discord.Rest;

public class RestUser : 
    RestEntity<Snowflake, IUserModel>, 
    IUser
{
    private readonly RestUserActor _actor;
    
    public RestUser(IUserModel model, DiscordRestClient client) : base(model, client)
    {
        _actor = client.Users[model.Id];
    }

    public ValueTask<IUser> GetAsync(RequestOptions options = default) => _actor.GetAsync(options);

    public string Username => throw new NotImplementedException();

    public short? Discriminator => throw new NotImplementedException();

    public string? GlobalName => throw new NotImplementedException();

    public string? AvatarId => throw new NotImplementedException();

    public string? BannerId => throw new NotImplementedException();

    public bool IsBot => throw new NotImplementedException();

    public bool IsSystem => throw new NotImplementedException();

    public Color? AccentColor => throw new NotImplementedException();

    public UserFlags Flags => throw new NotImplementedException();

    public UserFlags PublicFlags => throw new NotImplementedException();
}