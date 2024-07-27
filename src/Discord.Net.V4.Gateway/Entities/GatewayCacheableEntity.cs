using Discord.Gateway.Cache;
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
    where TModel : IEntityModel<TId>
    where TSelf :
        GatewayCacheableEntity<TSelf, TId, TModel, TIdentity>,
        IContextConstructable<TSelf, TModel, IPathable, DiscordGatewayClient>
{
    public void Dispose()
    {
        // TODO release managed resources here
    }

    public async ValueTask DisposeAsync()
    {
        // TODO release managed resources here
    }

    protected GatewayCacheableEntity(DiscordGatewayClient discord, TId id) : base(discord, id)
    {
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
