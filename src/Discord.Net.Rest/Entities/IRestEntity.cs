using Discord.Models;
using Discord.Rest.Api;

namespace Discord.Rest;

public interface IRestEntity :
    IRestClientProvider
{
}

public interface IRestEntity<out TId> : IRestEntity, IEntity<TId> where TId : IEquatable<TId>;

public interface IRestEntity<out TId, TModel> :
    IRestEntity<TId>,
    IEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IModel;

public interface IRestEntity<out TSelf, out TId, TModel> :
    IRestEntity<TId, TModel>
    where TId : IEquatable<TId>
    where TModel : IModel
    where TSelf : IRestEntity<TSelf, TId, TModel>
{
    static abstract TSelf Create(DiscordRestClient client, TModel model);
}

public interface IRestPipelineEntity<TSelf> : IRestEntity
    where TSelf : IRestPipelineEntity<TSelf>
{
    static abstract IRestApiPipeline<TSelf> FromPipeline(IRestApiPipeline<HttpResponseMessage> pipeline);
}