using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Discord.Models;

namespace Discord.Rest.Api;

public interface IRestApiPipeline<TOut>
{
    ValueTask<TOut> RunAsync(DiscordRestClient client, RequestOptions options);
}

file sealed record RequestPipeline<TOperation>(
    TOperation Operation,
    RequestBody? Body
) : IRestApiPipeline<HttpResponseMessage>
    where TOperation : IOperation
{
    public async ValueTask<HttpResponseMessage> RunAsync(DiscordRestClient client, RequestOptions options)
    {
        return await client.Api
            .ExecuteRequestAsync(
                Operation,
                Body,
                options
            );
    }
}

file sealed record TransformPipeline<TIn, TOut>(
    IRestApiPipeline<TIn> Previous,
    Func<TIn, DiscordRestClient, RequestOptions, ValueTask<TOut>> Transform
) : IRestApiPipeline<TOut>
{
    public async ValueTask<TOut> RunAsync(DiscordRestClient client, RequestOptions options)
        => await Transform(await Previous.RunAsync(client, options), client, options);
}

public static class PipelineExtensions
{
    [OverloadResolutionPriority(2)]
    public static IRestApiPipeline<TModel> AsPipeline<TOperation, TModel, TParams>(
        this Routes.Expand<TOperation, Routes.Expand<Routes.Out<TModel>, Routes.In<TParams>>> operation,
        TParams body
    ) 
        where TOperation : IOperation, Routes.Out<TModel>, Routes.In<TParams>
        where TParams : IParametersModel
        => new RequestPipeline<TOperation>((TOperation) operation, new RequestBody.Json(body))
            .Deserialize<TModel>()!;
    
    [OverloadResolutionPriority(1)]
    public static IRestApiPipeline<TModel> AsPipeline<TOperation, TModel>(
        this Routes.Expand<TOperation, Routes.Out<TModel>> operation,
        RequestBody? body = null
    ) where TOperation : IOperation, Routes.Out<TModel>
        => new RequestPipeline<TOperation>((TOperation) operation, body)
            .Deserialize<TModel>()!;

    public static IRestApiPipeline<HttpResponseMessage> AsPipeline<TOperation>(
        this TOperation operation,
        RequestBody? body = null
    ) where TOperation : IOperation
        => new RequestPipeline<TOperation>(operation, body);

    public static IRestApiPipeline<TOut> Map<TIn, TOut>(
        this IRestApiPipeline<TIn> previous,
        Func<TIn, TOut> transform
    ) => new TransformPipeline<TIn, TOut>(previous, (x, _, _) => ValueTask.FromResult(transform(x)));

    public static IRestApiPipeline<TOut> Map<TIn, TOut>(
        this IRestApiPipeline<TIn> previous,
        Func<DiscordRestClient, TIn, TOut> transform
    ) => new TransformPipeline<TIn, TOut>(previous, (previous, client, _) => ValueTask.FromResult(transform(client, previous)));

    public static IRestApiPipeline<TOut> Map<TIn, TOut>(
        this IRestApiPipeline<TIn> previous,
        Func<TIn, DiscordRestClient, RequestOptions, ValueTask<TOut>> transform
    ) => new TransformPipeline<TIn, TOut>(previous, transform);

    public static IRestApiPipeline<TOut> Map<TIn, TOut>(
        this IRestApiPipeline<TIn> previous,
        Func<TIn, DiscordRestClient, RequestOptions, TOut> transform
    ) => new TransformPipeline<TIn, TOut>(previous,
        (previous, client, options) => ValueTask.FromResult(transform(previous, client, options)));

    public static IRestApiPipeline<TModel?> Deserialize<TModel>(
        this IRestApiPipeline<HttpResponseMessage> previous
    ) => previous.Map(async (response, client, options) =>
        {
            if (response.Content.Headers.ContentLength is null or 0) return default;

            if (response.Content.Headers.ContentType?.MediaType is not "application/json")
                return default;

            return (TModel?) await JsonSerializer.DeserializeAsync(
                await response.Content.ReadAsStreamAsync(),
                client.JsonContext.GetTypeInfo(typeof(TModel))!,
                options.CancellationToken
            );
        }
    );
}