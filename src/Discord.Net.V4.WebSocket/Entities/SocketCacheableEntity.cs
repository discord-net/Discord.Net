using Discord.WebSocket.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public abstract class SocketCacheableEntity<TId, TModel> : SocketEntity<TId>, ICacheableEntity<TId, TModel>
        where TId : IEquatable<TId>
        where TModel : IEntityModel<TId>
    {
        internal SocketCacheableEntity(DiscordSocketClient discord, TId id)
            : base(discord, id)
        {
        }

        internal abstract object Clone();
        internal abstract void DisposeClone();

        internal abstract TModel GetModel();
        internal abstract void Update(TModel model);

        TModel ICacheableEntity<TId, TModel>.GetModel() => GetModel();
        void ICacheableEntity<TId, TModel>.Update(TModel model) => Update(model);
        void IScopedClonable.DisposeClone() => DisposeClone();
        object ICloneable.Clone() => Clone();
    }

    public abstract class SocketCacheableEntity<TId> : SocketCacheableEntity<TId, IEntityModel<TId>>, ICacheableEntity<TId>
        where TId : IEquatable<TId>
    {
        internal SocketCacheableEntity(DiscordSocketClient discord, TId id)
            : base(discord, id)
        {
        }
    }
}
