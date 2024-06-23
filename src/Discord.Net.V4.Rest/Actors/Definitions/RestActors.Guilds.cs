using Discord.Models;
using Discord.Rest.Guilds;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static RestPagedIndexableActor<RestLoadableBanActor, ulong, RestBan, IBanModel> Bans(DiscordRestClient client, ulong guildId)
    {
        // TOOD: user-configurable pagination
        return null!;
        // return new RestPagedIndexableActor<RestLoadableBanActor, ulong, RestBan, IBanModel>(
        //     client,
        //     banId => new RestLoadableBanActor(client, guildId, banId),
        //     Routes.GetGuildBans()
        // );
    }
}
