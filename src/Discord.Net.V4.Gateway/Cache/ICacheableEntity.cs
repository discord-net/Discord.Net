using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.Cache
{
    public interface ICacheableEntity<TId, TModel> : ICacheableEntity<TId>, ICacheUpdatable<TId, TModel>, IEntity<TId>, IScopedClonable
        where TId : IEquatable<TId>
        where TModel : IEntityModel<TId>
    {
        new TModel GetModel();

        IEntityModel<TId> ICacheableEntity<TId>.GetModel()
            => GetModel();

        void ICacheableEntity<TId>.Update(IEntityModel<TId> model)
        {
            if(model is not TModel entityModel)
            {
                throw new InvalidCastException("The supplied entity model is not applicable for the target entity");
            }

            Update(entityModel);
        }


        void ICacheUpdatable<TId, TModel>.Update(TModel model, CacheOperation operation)
        {
            if (operation is CacheOperation.Create or CacheOperation.Update)
                Update(model);
        }
    }

    public interface ICacheableEntity<TId> : IEntity<TId>, IScopedClonable, IDisposable, IAsyncDisposable
        where TId : IEquatable<TId>
    {
        IEntityModel<TId> GetModel();

        void Update(IEntityModel<TId> model);

        void AcceptHandle(IEntityHandle handle);
        void DereferenceHandle(IEntityHandle handle);
    }
}
