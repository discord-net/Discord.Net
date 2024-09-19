using Discord.Models;
using MorseCode.ITask;

namespace Discord.Rest;

internal sealed class RestPagingProvider<TModel, TApiModel, TParams> : IPagingProvider<TModel>
    where TModel : IModel
    where TParams : class, IPagingParams<TParams, TApiModel>
    where TApiModel : class
{
    private readonly DiscordRestClient _client;
    private readonly TParams _pageParams;
    private readonly Func<TApiModel, IEnumerable<TModel>?> _mapper;
    private readonly RequestOptions? _options;
    private readonly CancellationToken _token;
    private readonly IPathable _path;
    
    private TApiModel? _lastRequest;
    
    public RestPagingProvider(
        DiscordRestClient client, 
        TParams pageParams, 
        Func<TApiModel, IEnumerable<TModel>?> mapper,
        IPathable? path = null,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        _client = client;
        _pageParams = pageParams;
        _mapper = mapper;
        _options = options;
        _token = token;
        _path = path ?? IPathable.Empty;
    }
    
    
    public async ITask<IEnumerable<TModel>?> NextAsync()
    {
        _token.ThrowIfCancellationRequested();
        
        var route = TParams.GetRoute(_pageParams, _path, _lastRequest);

        if (route is null) return null;

        var apiResult = _lastRequest = await _client.RestApiClient.ExecuteAsync(
            route,
            _options,
            _token
        );

        return apiResult is null 
            ? null 
            : _mapper(apiResult);
    }
}