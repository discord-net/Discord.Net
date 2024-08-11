using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.GetChannelInvites))]
public partial interface IChannelInvite : IInvite, IChannelInviteActor;
