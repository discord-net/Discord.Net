using Discord.API;
using Discord.Models;
using Discord.Rest;

namespace Discord.Rest;

public sealed class RestSelfUser : RestUser, ISelfUser
{
    public string Email
        => Model.Email!;

    public bool IsVerified
        => Model.Verified ?? false;

    public bool IsMfaEnabled
        => Model.MFAEnabled ?? false;

    public UserFlags Flags
        => (UserFlags?)Model.Flags ?? UserFlags.None;

    public PremiumType PremiumType
        => (PremiumType?)Model.Premium ?? PremiumType.None;

    public string Locale
        => Model.Locale!;

    internal RestSelfUser(DiscordRestClient client, IUserModel model)
        : base(client, model)
    { }

    internal new static RestSelfUser Create(DiscordRestClient client, IUserModel model) => new(client, model);

    public async Task ModifyAsync(Action<ModifySelfUserProperties> func, RequestOptions? options = null,
        CancellationToken token = default)
    {
        Update(
            await EntityUtils.ModifyAsync<User, ModifyCurrentUserParams, ModifySelfUserProperties>(
                Client,
                Routes.ModifyCurrentUser,
                func,
                options ?? Client.DefaultRequestOptions,
                token
            )
        );
    }
}
