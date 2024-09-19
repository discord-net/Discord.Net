using MorseCode.ITask;

namespace Discord.Rest;

public delegate ITask<TModel> ApiModelProviderDelegate<out TModel>(
    DiscordRestClient client,
    RequestOptions? options,
    CancellationToken token
);