using Discord.Models;

namespace Discord.Rest.Users;

public class RestUser(DiscordRestClient client, IUserModel model) : RestEntity<ulong>(client, model.Id), IUser
{
    protected virtual IUserModel Model { get; } = model;

    public string? AvatarId => Model.AvatarHash;

    public ushort Discriminator => ushort.Parse(Model.Discriminator);

    public string Username => Model.Username;

    public string? GlobalName => Model.GlobalName;

    public bool IsBot => Model.IsBot ?? false;

    public UserFlags PublicFlags => (UserFlags?)Model.PublicFlags ?? UserFlags.None;
}
