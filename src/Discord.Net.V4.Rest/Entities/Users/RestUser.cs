using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

public class RestUser : RestEntity<ulong>, IUser, IModeled<ulong, IUserModel>
{
    public string? AvatarId
        => Model.AvatarHash;

    public ushort Discriminator
        => ushort.Parse(Model.Discriminator);

    public string Username
        => Model.Username;

    public string? GlobalName
        => Model.GlobalName;

    public bool IsBot
        => Model.IsBot ?? false;

    public virtual bool IsWebhook => false;

    public UserFlags PublicFlags
        => Model.PublicFlags ?? UserFlags.None;

    public virtual IUserModel Model { get; protected set; }

    protected RestUser(DiscordRestClient client, IUserModel model)
        : base(client, model.Id)
    {
        Update(model);
    }

    internal static RestUser Create(DiscordRestClient client, IUserModel model)
    {
        return model.Id == client.CurrentUser.Id
            ? RestSelfUser.Create(client, model)
            : new RestUser(client, model);
    }

    [MemberNotNull(nameof(Model))]
    protected virtual void Update(IUserModel model)
    {
        Model = model;
    }
}
