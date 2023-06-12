using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    internal interface ICacheableEntity<TId, TModel> : ICacheableEntity<TId>, IEntity<TId>, IScopedClonable
        where TId : IEquatable<TId>
        where TModel : IEntityModel<TId>
    {
        new TModel GetModel();

        void Update(TModel model);

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
    }

    internal interface ICacheableEntity<TId> : IEntity<TId>, IScopedClonable
        where TId : IEquatable<TId>
    {
        IEntityModel<TId> GetModel();

        void Update(IEntityModel<TId> model);
    }
}
