using Discord.Models;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace Discord.Gateway;

public abstract class DispatchEvent<TPackage, TPayload>(DiscordGatewayClient client) :
    IDispatchEvent,
    IDispatchEventPackager<TPackage, TPayload>
    where TPackage : IDispatchPackage
    where TPayload : IGatewayPayloadData
{
    private readonly ILogger<DispatchEvent<TPackage, TPayload>> _logger =
        client.LoggerFactory.CreateLogger<DispatchEvent<TPackage, TPayload>>();
    
    private readonly struct DeferredOperation(Delegate source, InvocableEventHandler<TPackage>? handler = null)
    {
        public readonly InvocableEventHandler<TPackage>? Handler = handler;
        public readonly Delegate Source = source;

        public override string ToString()
        {
            return Handler is not null
                ? $"Deferred Subscribe Operation: '{Source}'"
                : $"Deferred Unsubscribe Operation: '{Source}'";
        }
    }

    public bool HasSubscribers => _handlers.Count > 0;
    public virtual bool RequiresPreparation => _deferredOperations.Count > 0;

    protected DiscordGatewayClient Client { get; } = client;

    private readonly Dictionary<Delegate, InvocableEventHandler<TPackage>> _handlers = new();
    private readonly Queue<DeferredOperation> _deferredOperations = new();

    public virtual void Prepare()
    {
        while (_deferredOperations.TryDequeue(out var operation))
        {
            _logger.LogDebug("{Self}: Preparation step: {Operation}", GetType(), operation);
            
            if (operation.Handler is not null)
                _handlers.Add(operation.Source, operation.Handler);
            else
                _handlers.Remove(operation.Source);
        }
    }

    public async ValueTask<IEnumerable<PreparedInvocableEventHandle>?> GetHandlersAsync(
        IGatewayPayloadData? payload,
        CancellationToken token)
    {
        if (payload is not TPayload ourPayload) return null;

        _logger.LogDebug("{Self}: Packaging {Payload}...", GetType(), typeof(TPayload));
        
        var package = await PackageAsync(ourPayload, token);

        if (package is null)
        {
            _logger.LogDebug("{Self}: No package created, returning", GetType());
            return null;
        }

        _logger.LogDebug("{Self}: Created package {Package}", GetType(), typeof(TPackage));
        
        // TODO: this capture can be alleviated probably
        return _handlers.Values.Select(x =>
            new PreparedInvocableEventHandle(token => x(package, token))
        );
    }

    protected void AddSubscriber<T>(T key, InvocableEventHandler<TPackage> handler)
        where T : Delegate
        => _deferredOperations.Enqueue(new(key, handler));

    protected void RemoveSubscriber<T>(T key)
        where T : Delegate
        => _deferredOperations.Enqueue(new(key));

    public abstract ValueTask<TPackage?> PackageAsync(TPayload? payload, CancellationToken token = default);

    public override string ToString()
    {
        return $"{GetType().Name} ({typeof(TPackage).Name} | {typeof(TPayload).Name})";
    }

    Type IDispatchEvent.PackageType => typeof(TPackage);
}

public interface IDispatchEvent
{
    bool RequiresPreparation { get; }
    bool HasSubscribers { get; }

    Type PackageType { get; }

    void Prepare();

    ValueTask<IEnumerable<PreparedInvocableEventHandle>?> GetHandlersAsync(IGatewayPayloadData? payload, CancellationToken token);
}
