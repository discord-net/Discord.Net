using System;
namespace Discord.Gateway
{
    public interface ICacheUpdatable<TId, TModel> : IEntity<TId>
        where TModel : IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        internal void Update(TModel model, CacheOperation operation);
    }
}

