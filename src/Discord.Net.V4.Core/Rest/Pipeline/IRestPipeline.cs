using Discord.Models;
using MorseCode.ITask;

namespace Discord.Rest.Pipeline;

public interface IRestPipeline<TOut>
{
    ValueTask<TOut> RunAsync(IDiscordClient client, CancellationToken token);
}

internal sealed record PipelineContext(
    IDiscordClient Client
)
{
    
}

file abstract record RestPipelineElement<T> : IRestPipeline<Optional<T>>
{
    protected abstract ValueTask<Optional<T>> RunAsync(
        PipelineContext context,
        CancellationToken token
    );

    public ValueTask<Optional<T>> RunAsync(IDiscordClient client, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}

file sealed record PreconditionPipeline<T>(
    IRestPipeline<T> Pipeline,
    Predicate<T> Precondition
) : IRestPipeline<Optional<T>>
{
    public async ValueTask<Optional<T>> RunAsync(IDiscordClient client, CancellationToken token)
    {
        var previous = await Pipeline.RunAsync(client, token);
        return previous.SomeWhen(Precondition);
    }
}

file sealed record ContinuationPipeline<T>(
    IRestPipeline<T> Pipeline,
    Func<T, CancellationToken, ValueTask> Continuation
) : IRestPipeline<T>
{
    public async ValueTask<T> RunAsync(IDiscordClient client, CancellationToken token)
    {
        var previous = await Pipeline.RunAsync(client, token);
        await Continuation(previous, token);
        return previous;
    }
}

file sealed record TransformPipeline<T, U>(
    IRestPipeline<T> Pipeline,
    Func<T, IDiscordClient, CancellationToken, ValueTask<U>> Transform
) : IRestPipeline<U>
{
    public async ValueTask<U> RunAsync(IDiscordClient client, CancellationToken token)
        => await Transform(await Pipeline.RunAsync(client, token), client, token);
}

public static class RestPipelineExtensions
{
    public static ValueTask<T> RunAsync<T>(this IRestPipeline<T> pipeline, IClientProvider provider, CancellationToken token)
        => pipeline.RunAsync(provider.Client, token);
    
    public static RequestPipeline<TRoute> AsPipeline<TRoute>(this TRoute route, RequestOptions? options = null)
        where TRoute : IRouteOperation<TRoute>
        => new(route, options);

    public static RequestPipeline<TRoute, TParams> AsPipeline<TRoute, TParams>(
        this TRoute route,
        TParams? args,
        RequestOptions? options = null
    )
        where TRoute : IRouteOperation<TRoute>
        where TParams : IRequestParams
        => new(route, args, options);

    public static DeserializePipeline<T> Deserialize<T>(this IRestPipeline<IRestResponse> responsePipeline)
        => new(responsePipeline);

    public static IRestPipeline<Optional<T>> Maybe<T>(this IRestPipeline<T> pipeline, Predicate<T> predicate)
        => new PreconditionPipeline<T>(pipeline, predicate);

    public static IRestPipeline<T?> AsNullable<T>(this IRestPipeline<Optional<T>> pipeline)
        => Transform(pipeline, (opt, _) => ValueTask.FromResult(opt.IsSpecified ? opt.Value : default));
    
    public static IRestPipeline<Optional<T>> IfNotNull<T>(this IRestPipeline<T?> pipeline)
        => Maybe(pipeline, x => x is not null)!;

    public static IRestPipeline<T> Required<T>(this IRestPipeline<T?> pipeline)
        => Transform(
            pipeline,
            (x, _) =>
            {
                if (x is null) throw new NullReferenceException("Value is null");
                return ValueTask.FromResult(x);
            })!;
    
    public static IRestPipeline<T> Required<T>(this IRestPipeline<Optional<T>> pipeline)
        => Transform(
            pipeline,
            (x, _) =>
            {
                if (!x.IsSpecified) throw new InvalidOperationException("Required value is not specified");
                return ValueTask.FromResult(x.Value);
            })!;

    public static IRestPipeline<Optional<T>> Continue<T>(
        this IRestPipeline<Optional<T>> pipeline,
        Func<T, CancellationToken, ValueTask> Continuation)
        => new ContinuationPipeline<Optional<T>>(
            pipeline,
            async (val, token) =>
            {
                if (val.IsSpecified)
                    await Continuation(val.Value, token);
            }
        );
    
    public static IRestPipeline<T> Continue<T>(
        this IRestPipeline<T> pipeline,
        Func<T, CancellationToken, ValueTask> Continuation)
        => new ContinuationPipeline<T>(
            pipeline,
            Continuation
        );

    public static IRestPipeline<T> Construct<T, TModel>(this IRestPipeline<TModel> pipeline, T? type)
        where T : IModelConstructable<T, TModel>
        => Transform(pipeline, static (model, client, _) => ValueTask.FromResult(T.Construct(client, model)));
    
    public static IRestPipeline<U> Transform<T, U>(this IRestPipeline<T> pipeline,
        Func<T, U> transform)
        => new TransformPipeline<T, U>(pipeline, (value, _, _) => ValueTask.FromResult(transform(value)));

    public static IRestPipeline<U> Transform<T, U>(this IRestPipeline<T> pipeline,
        Func<T, IDiscordClient, CancellationToken, ValueTask<U>> transform)
        => new TransformPipeline<T, U>(pipeline, transform);
    
    public static IRestPipeline<U> Transform<T, U>(this IRestPipeline<T> pipeline,
        Func<T, CancellationToken, ValueTask<U>> transform)
        => new TransformPipeline<T, U>(pipeline, (value, _,  token) => transform(value, token));
    
    public static IRestPipeline<U> Transform<T, U>(this IRestPipeline<T> pipeline,
        Func<T, CancellationToken, ITask<U>> transform)
        => new TransformPipeline<T, U>(pipeline, async (value, _, token) => await transform(value, token));
    
    public static IRestPipeline<U> Transform<T, U>(this IRestPipeline<T> pipeline,
        Func<T, CancellationToken, Task<U>> transform)
        => new TransformPipeline<T, U>(pipeline, async (value, _, token) => await transform(value, token));
    public static IRestPipeline<U> Transform<T, U>(this IRestPipeline<T> pipeline,
        Func<T, IDiscordClient, CancellationToken, Task<U>> transform)
        => new TransformPipeline<T, U>(pipeline, async (value, client, token) => await transform(value, client, token));

    public static IRestPipeline<Optional<U>> Transform<T, U>(this IRestPipeline<Optional<T>> pipeline,
        Func<T, CancellationToken, ValueTask<U>> transform)
        => new TransformPipeline<Optional<T>, Optional<U>>(
            pipeline,
            async (val, _, token) =>
            {
                if (val.IsSpecified)
                    return Optional.Some(await transform(val.Value, token));

                return default;
            }
        );
    
    public static IRestPipeline<Optional<U>> Transform<T, U>(this IRestPipeline<Optional<T>> pipeline,
        Func<T, CancellationToken, ITask<U>> transform)
        => new TransformPipeline<Optional<T>, Optional<U>>(
            pipeline,
            async (val, _, token) =>
            {
                if (val.IsSpecified)
                    return Optional.Some(await transform(val.Value, token));

                return default;
            }
        );
}