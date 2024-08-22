using Discord.Models;
using Discord.Rest;

namespace Discord;

[Deletable(nameof(Routes.DeleteAllReactionsForEmoji))]
public partial interface IReactionActor :
    IActor<DiscordEmojiId, IReaction>,
    IChannelRelationship<IMessageChannelTrait, IMessageChannel>,
    IMessageRelationship,
    IEntityProvider<IReaction, IReactionModel>
{
    ICurrentUserActor.BackLink<IReactionActor> CurrentUser { get; }
    
    IUserActor.BackLink<IReactionActor>.Paged<PageUserBasicReactionsParams>.Indexable Users { get; }
    
    IUserActor.BackLink<IReactionActor>.Paged<PageUserSuperReactionsParams>.Indexable SuperReactionUsers { get; }
    
    [OnVertex]
    private static Task AddAsync(
        ICurrentUserActor.BackLink<IReactionActor> target,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        return target.Client.RestApiClient.ExecuteAsync(
            Routes.CreateReaction(
                target.Source.Channel.Id,
                target.Source.Message.Id,
                target.Source.Id
            ),
            options ?? target.Client.DefaultRequestOptions,
            token
        );
    }
    
    [BackLink<IMessageActor>]
    private static Task RemoveAllAsync(
        IMessageActor message,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        return message.Client.RestApiClient.ExecuteAsync(
            Routes.DeleteAllReactions(message.Channel.Id, message.Id),
            options ?? message.Client.DefaultRequestOptions,
            token
        );
    }
    
    [OnVertex]
    private static Task RemoveAsync(
        IUserActor.BackLink<IReactionActor> target,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        return target.Client.RestApiClient.ExecuteAsync(
            target is ICurrentUserActor
                ? Routes.DeleteOwnReaction(target.Source.Channel.Id, target.Source.Message.Id, target.Source.Id)
                : Routes.DeleteUserReaction(target.Source.Channel.Id, target.Source.Message.Id, target.Source.Id, target.Id),
            options ?? target.Client.DefaultRequestOptions,
            token
        );
    }
}