namespace Discord;
#pragma warning disable CA2252

public interface IConstructable<out TSelf, in TModel> : IConstructable<TSelf, TModel, IDiscordClient>
    where TSelf : IConstructable<TSelf, TModel>;

public interface IConstructable<out TSelf, in TModel, in TClient>
    where TSelf : IConstructable<TSelf, TModel, TClient>
    where TClient : IDiscordClient
{
    static abstract TSelf Construct(TClient client, TModel model);
}

public interface IContextConstructable<out TSelf, in TModel, in TContext> :
    IContextConstructable<TSelf, TModel, TContext, IDiscordClient>
    where TSelf : IContextConstructable<TSelf, TModel, TContext>;

public interface IContextConstructable<out TSelf, in TModel, in TContext, in TClient>
    where TSelf : IContextConstructable<TSelf, TModel, TContext, TClient>
    where TClient : IDiscordClient
{
    static abstract TSelf Construct(TClient client, TContext context, TModel model);
}

#pragma warning restore CA2252
