namespace Discord;

public interface IConstructable<out TSelf, in TActor, in TModel> : IConstructable<TSelf, TActor, TModel, IDiscordClient>
    where TSelf : IConstructable<TSelf, TActor, TModel>;

public interface IConstructable<out TSelf, in TActor, in TModel, in TClient>
    where TSelf : IConstructable<TSelf, TActor, TModel, TClient>
    where TClient : IDiscordClient
{
    static abstract TSelf Construct(TClient client, TActor actor, TModel model);
}

public interface IModelConstructable<out TSelf, in TModel> : IModelConstructable<TSelf, TModel, IDiscordClient>
    where TSelf : IModelConstructable<TSelf, TModel>;

public interface IModelConstructable<out TSelf, in TModel, in TClient>
    where TSelf : IModelConstructable<TSelf, TModel, TClient>
    where TClient : IDiscordClient
{
    static abstract TSelf Construct(TClient client, TModel model);
}