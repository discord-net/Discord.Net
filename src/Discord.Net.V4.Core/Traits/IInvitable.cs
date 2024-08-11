using Discord.Models;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
// ReSharper disable UnusedTypeParameter

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class InvitableAttribute<TParams, TInvite>(string route) : Attribute
    where TInvite : IInvite
    where TParams : class;

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class InvitableAttribute<TParams>(string route) : Attribute
    where TParams : class;

#pragma warning restore CS9113 // Parameter is unread.
// ReSharper restore UnusedTypeParameter

public interface IInvitable<TSelf, TInvite, TId, in TParams, TApi> :
    IInvitable<TParams>,
    IPathable,
    IEntityProvider<TInvite, IInviteModel>,
    IIdentifiable<TId>
    where TId : IEquatable<TId>
    where TParams : class, IEntityProperties<TApi>
    where TApi : class
    where TSelf : IInvitable<TSelf, TInvite, TId, TParams, TApi>
    where TInvite : class, IInvite
{
    [return: TypeHeuristic<IEntityProvider<IInvite, IInviteModel>>(nameof(CreateEntity))]
    new Task<TInvite> CreateInviteAsync(
        TParams args,
        RequestOptions? options = null,
        CancellationToken token = default)
        => CreateInviteInternalAsync(
            Client,
            TSelf.CreateInviteRoute(this, Id, args.ToApiModel()),
            this,
            options,
            token
        );

    [return: TypeHeuristic<IEntityProvider<IInvite, IInviteModel>>(nameof(CreateEntity))]
    internal static async Task<TInvite> CreateInviteInternalAsync(
        IDiscordClient client,
        IApiInOutRoute<TApi, IInviteModel> route,
        IEntityProvider<TInvite, IInviteModel> provider,
        RequestOptions? options,
        CancellationToken token)
    {
        var model = await client.RestApiClient.ExecuteRequiredAsync(
            route,
            options ?? client.DefaultRequestOptions,
            token
        );

        return provider.CreateEntity(model);
    }

    static abstract IApiInOutRoute<TApi, IInviteModel> CreateInviteRoute(IPathable path, TId id, TApi args);

    async Task<IInvite> IInvitable<TParams>.CreateInviteAsync(TParams args, RequestOptions? options,
        CancellationToken token)
        => await CreateInviteAsync(args, options, token);
}

public interface IInvitable<in TParams>
{
    Task<IInvite> CreateInviteAsync(
        TParams args,
        RequestOptions? options = null,
        CancellationToken token = default
    );
}
