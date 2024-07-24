using Discord.Models;

namespace Discord.Rest.Guilds;

[ExtendInterfaceDefaults]
public class RestPartialGuild(DiscordRestClient client, IPartialGuildModel model) :
    RestEntity<ulong>(client, model.Id),
    IPartialGuild,
    IConstructable<RestPartialGuild, IPartialGuildModel, DiscordRestClient>
{
    public string Name => Model.Name;

    public string? SplashId => Model.SplashId;

    public string? BannerId => Model.BannerId;

    public string? Description => Model.Description;

    public string? IconId => Model.IconId;

    public GuildFeatures? Features => new(Model.Features ?? []);

    public VerificationLevel? VerificationLevel =>
        (VerificationLevel?)Model.VerificationLevel;

    public string? VanityUrlCode => Model.VanityUrlCode;

    public NsfwLevel? NsfwLevel => (NsfwLevel?)Model.NsfwLevel;

    public int? PremiumSubscriptionCount => Model.PremiumSubscriptionCount;

    internal virtual IPartialGuildModel Model { get; } = model;

    public static RestPartialGuild Construct(DiscordRestClient client, IPartialGuildModel model)
    {
        return model switch
        {
            IGuildModel guild => RestGuild.Construct(client, guild),
            _ => new RestPartialGuild(client, model)
        };
    }

    public virtual IPartialGuildModel GetModel()
        => Model;
}
