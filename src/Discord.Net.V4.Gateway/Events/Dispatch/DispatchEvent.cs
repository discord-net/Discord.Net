using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Gateway;

public abstract class DispatchEvent<TPackage, TPayload>(DiscordGatewayClient client) :
    IDispatchEvent,
    IDispatchEventPackager<TPackage, TPayload>
    where TPackage : IDispatchPackage
    where TPayload : IGatewayPayloadData
{
    private readonly struct DeferredOperation(Delegate source, InvocableEventHandler<TPackage>? handler = null)
    {
        public readonly InvocableEventHandler<TPackage>? Handler = handler;
        public readonly Delegate Source = source;
    }

    public bool HasSubscribers => _handlers.Count > 0;
    public bool RequiresPreparation => _deferredOperations.Count > 0;

    protected DiscordGatewayClient Client { get; } = client;

    private readonly Dictionary<Delegate, InvocableEventHandler<TPackage>> _handlers = new();
    private readonly Queue<DeferredOperation> _deferredOperations = new();

    public virtual void Prepare()
    {
        while (_deferredOperations.TryDequeue(out var operation))
        {
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

        var package = await PackageAsync(ourPayload, token);

        if (package is null) return null;

        // TODO: this capture can be alleviated probably
        return _handlers.Values.Select(x =>
            Prepare(x, package)
        );
    }

    private PreparedInvocableEventHandle Prepare(InvocableEventHandler<TPackage> handler, TPackage package)
        => token => handler(package, token);

    protected void AddSubscriber<T>(T key, InvocableEventHandler<TPackage> handler)
        where T : Delegate
        => _deferredOperations.Enqueue(new(key, handler));

    protected void RemoveSubscriber<T>(T key)
        where T : Delegate
        => _deferredOperations.Enqueue(new(key));

    public abstract ValueTask<TPackage?> PackageAsync(TPayload? payload, CancellationToken token = default);

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
