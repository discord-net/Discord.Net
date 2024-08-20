namespace Discord.Rest;

public delegate Task<TModel> ApiModelProviderDelegate<TModel>(
    DiscordRestClient client,
    RequestOptions? options,
    CancellationToken token
);