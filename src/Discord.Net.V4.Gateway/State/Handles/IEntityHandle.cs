using Discord.Gateway.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

public interface IEntityHandle<out TId, out TEntity> :
    IIdentifiable<TId>,
    IAsyncDisposable
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    TEntity Entity { get; }
}
