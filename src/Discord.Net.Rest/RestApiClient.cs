using System.Net.Http.Headers;
using System.Net.Http.Json;
using Discord.Models.Json;
using Discord.Models.Rest.Api;
using Discord.Rest.Api;

namespace Discord.Rest;

public sealed class RestApiClient
{
    private readonly DiscordRestClient _discordClient;
    private readonly HttpClient _httpClient;

    public RestApiClient(
        DiscordRestClient client
    )
    {
        _discordClient = client;
        _httpClient = new();
    }

    private async Task<HttpResponseMessage> ExecuteRequestAsync<T>(
        T operation,
        RequestBody? body,
        RequestOptions options
    ) where T : IOperation
    {
        var request = new HttpRequestMessage(
            ToHttpMethod(T.Method),
            operation.Format()
        )
        {
            Content = EncodeBody(body)
        };

        AddAuthorizationHeaders(T.AuthenticationScheme, request);
        
        if (options.AuditLogReason is not null)
            request.Headers.Add("X-Audit-Log-Reason", Uri.EscapeDataString(options.AuditLogReason));

        // TODO: ratelimits

        var response = await _httpClient.SendAsync(request);

        // TODO: ratelimits
        
        return response;
    }

    private void AddAuthorizationHeaders(AuthenticationScheme scheme, HttpRequestMessage request)
    {
        switch (_discordClient.Config.Token.Type)
        {
            case TokenType.Bot when scheme.HasFlag(AuthenticationScheme.BotToken):
                request.Headers.Authorization = new AuthenticationHeaderValue("Bot", _discordClient.Config.Token.Value);
                break;
            case TokenType.Bearer when scheme.HasFlag(AuthenticationScheme.BearerToken):
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", _discordClient.Config.Token.Value);
                break;
            default:
                if (scheme is not AuthenticationScheme.None)
                    throw new InvalidOperationException("Missing authorization");
                break;
        }
    }

    private HttpContent? EncodeBody(RequestBody? body)
    {
        if (body is null) return null;

        switch (body)
        {
            case RequestBody.Json(var model):
                var jsonModel = DiscordJsonContext.AsJsonModel(model);

                // TODO: this may be prone to failing if sub types of the model interfaces are supplied.
                if (_discordClient.JsonContext.GetTypeInfo(model.GetType()) is not { } typeInfo)
                    throw new InvalidOperationException($"Missing type info for '{model.GetType()}'");

                return JsonContent.Create(jsonModel, typeInfo);
            default:
                throw new NotImplementedException();
        }
    }

    private static HttpMethod ToHttpMethod(RequestMethod method)
    {
        return method switch
        {
            RequestMethod.Get => HttpMethod.Get,
            RequestMethod.Put => HttpMethod.Put,
            RequestMethod.Post => HttpMethod.Post,
            RequestMethod.Delete => HttpMethod.Delete,
            RequestMethod.Patch => HttpMethod.Patch,
            _ => throw new InvalidOperationException()
        };
    }
}