using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public abstract class SocketCacheableEntity<TId, TModel> : SocketCacheableEntity<TId>, ICacheableEntity<TId, TModel>
        where TId : IEquatable<TId>
        where TModel : IEntityModel<TId>
    {
        protected abstract TModel Model { get; }

        internal SocketCacheableEntity(DiscordSocketClient discord, TId id)
            : base(discord, id)
        {
        }

        internal virtual TModel GetModel()
            => Model;

        internal abstract void Update(TModel model);

        internal override void Update(IEntityModel<TId> model)
        {
            if (model is TModel genericModel)
                Update(genericModel);
        }

        internal override IEntityModel<TId> GetGenericModel()
            => GetModel();

        TModel ICacheableEntity<TId, TModel>.GetModel() => GetModel();
        void ICacheableEntity<TId, TModel>.Update(TModel model) => Update(model);
    }

    public abstract class SocketCacheableEntity<TId> : SocketEntity<TId>, ICacheableEntity<TId>
        where TId : IEquatable<TId>
    {
        internal readonly HashSet<IEntityHandle> Handles;

        internal SocketCacheableEntity(DiscordSocketClient discord, TId id)
            : base(discord, id)
        {
            Handles = new();
        }

        internal void AcceptHandle(IEntityHandle handle)
           => Handles.Add(handle);

        internal void DereferenceHandle(IEntityHandle handle)
            => Handles.Remove(handle);

        ~SocketCacheableEntity()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);

            foreach (var handle in Handles)
            {
                handle.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);

            foreach (var handle in Handles)
            {
                await handle.DisposeAsync();
            }
        }

        internal abstract object Clone();
        internal abstract void DisposeClone();
        internal abstract IEntityModel<TId> GetGenericModel();
        internal abstract void Update(IEntityModel<TId> model);

        IEntityModel<TId> ICacheableEntity<TId>.GetModel() => GetModel();
        void ICacheableEntity<TId>.Update(IEntityModel<TId> model) => Update(model);
        void ICacheableEntity<TId>.AcceptHandle(IEntityHandle handle) => AcceptHandle(handle);
        void ICacheableEntity<TId>.DereferenceHandle(IEntityHandle handle) => DereferenceHandle(handle);
        void IScopedClonable.DisposeClone() => DisposeClone();
        object ICloneable.Clone() => Clone();
    }
}
