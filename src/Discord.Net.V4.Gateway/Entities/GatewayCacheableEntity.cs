using Discord.Gateway.Cache;
using Discord.Gateway.State;
using Discord.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

public abstract class GatewayCacheableEntity<TSelf, TId, TModel, TIdentity> :
    GatewayEntity<TId>,
    ICacheableEntity<TSelf, TId, TModel>
    where TId : IEquatable<TId>
    where TModel : class, IEntityModel<TId>
    where TSelf :
        GatewayCacheableEntity<TSelf, TId, TModel, TIdentity>,
        IStoreProvider<TId, TModel>,
        IContextConstructable<TSelf, TModel, ICacheConstructionContext<TId, TSelf>, DiscordGatewayClient>
{
    private IEntityHandle<TId, TSelf>? _implicitHandle;

    ~GatewayCacheableEntity()
    {
        Client.StateController.EnqueueEntityDestruction<TId, TSelf, TModel>(Id);
    }

    public async ValueTask DisposeAsync()
    {
        if (_implicitHandle is not null)
            await _implicitHandle.DisposeAsync();

        _implicitHandle = null;
    }

    protected GatewayCacheableEntity(
        DiscordGatewayClient discord,
        TId id,
        IEntityHandle<TId, TSelf>? implicitHandle
        ) : base(discord, id)
    {
        _implicitHandle = implicitHandle;
    }


    public abstract TModel GetModel();

    public abstract ValueTask UpdateAsync(TModel model, bool updateCache = true, CancellationToken token = default);
}

// public abstract class GatewayCacheableEntity<TId> : GatewayEntity<TId>, ICacheableEntity<TId>
//     where TId : IEquatable<TId>
// {
//     internal readonly HashSet<IEntityHandle> Handles;
//
//     internal GatewayCacheableEntity(DiscordGatewayClient discord, TId id)
//         : base(discord, id)
//     {
//         Handles = new();
//     }
//
//     internal void AcceptHandle(IEntityHandle handle)
//         => Handles.Add(handle);
//
//     internal void DereferenceHandle(IEntityHandle handle)
//         => Handles.Remove(handle);
//
//     ~GatewayCacheableEntity()
//     {
//         Dispose();
//     }
//
//     public void Dispose()
//     {
//         GC.SuppressFinalize(this);
//
//         foreach (var handle in Handles)
//         {
//             handle.Dispose();
//         }
//     }
//
//     public async ValueTask DisposeAsync()
//     {
//         GC.SuppressFinalize(this);
//
//         foreach (var handle in Handles)
//         {
//             await handle.DisposeAsync();
//         }
//     }
//
//     internal abstract object Clone();
//     internal abstract void DisposeClone();
//     internal abstract IEntityModel<TId> GetGenericModel();
//     internal abstract void Update(IEntityModel<TId> model);
//
//     IEntityModel<TId> ICacheableEntity<TId>.GetModel() => GetModel();
//     void ICacheableEntity<TId>.Update(IEntityModel<TId> model) => Update(model);
//     void ICacheableEntity<TId>.AcceptHandle(IEntityHandle handle) => AcceptHandle(handle);
//     void ICacheableEntity<TId>.DereferenceHandle(IEntityHandle handle) => DereferenceHandle(handle);
//     void IScopedClonable.DisposeClone() => DisposeClone();
//     object ICloneable.Clone() => Clone();
// }
