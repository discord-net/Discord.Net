using Discord.Models;

namespace Discord;

public interface IGuildActor :
    IActor<Snowflake, IGuild>,
    ILoadable<IGuild>,
    IModifiable<IModifyGuildParams, IGuild>
{
    IGuildChannelsLink Channels { get; }
    IMembersLink Members { get; }
    IRolesLink Roles { get; }
    IBansLink Bans { get; }
    IGuildVoiceRegionsLink VoiceRegions { get; }
    IGuildInvitesLink Invites { get; }

    Task<IPruneCountResponseModel> GetPruneCountAsync(
        IPruneCountParams? parameters = null,
        RequestOptions options = default
    );

    Task<IPruneCountResponseModel?> BeginPruneAsync(
        IBeginPruneParams? parameters = null,
        RequestOptions options = default
    );
}