using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.GetGuildInvites))]
public partial interface IGuildInvite : IInvite, IGuildInviteActor
{
    IGuildScheduledEventActor? GuildScheduledEvent { get; }
}
