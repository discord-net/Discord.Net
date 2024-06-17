namespace Discord.Rest.Users;

[ExtendInterfaceDefaults(typeof(ISelfUserActor), typeof(ISelfUser))]
public partial class RestSelfUser(DiscordRestClient client, Models.IUserModel model) : RestUser(client, model), ISelfUser
{
    public string Email => Model.Email!;

    public bool IsVerified => Model.Verified ?? false;

    public bool IsMfaEnabled => Model.MFAEnabled ?? false;

    public UserFlags Flags => (UserFlags?)Model.Flags ?? UserFlags.None;

    public PremiumType PremiumType => (PremiumType?)Model.Premium ?? PremiumType.None;

    public string Locale => Model.Locale!;
}
