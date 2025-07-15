using System.Net;

namespace Discord.Rest.Pipeline;

public record DeserializePipeline<T>(
    IRestPipeline<IRestResponse> Previous
) : IRestPipeline<T?>
{
    public bool ThrowOn404 { get; init; }
    public bool Required { get; init; }
    
    public async ValueTask<T?> RunAsync(IDiscordClient client, CancellationToken token)
    {
        var response = await Previous.RunAsync(client, token);
        
        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            if(ThrowOn404 || response.Options.ThrowOn404)
                throw new NotFoundException(response.Route, response.Options);

            return default;
        }

        if (Required && !response.HasContent)
            throw new DiscordHttpException(response.Route, response.Options, "Missing content");

        return await response.DeserializeAsync<T?>(token);
    }
}