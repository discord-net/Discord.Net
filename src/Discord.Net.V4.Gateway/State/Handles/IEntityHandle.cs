using Discord.Gateway.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

public interface IEntityHandle<out TId, out TEntity> :
    IEntityHandle<TId>
    where TEntity : class, IEntity<TId>
    where TId : IEquatable<TId>
{
    TEntity Entity { get; }
}

public interface IEntityHandle<out TId> :
    IIdentifiable<TId>,
    IDisposable
    where TId : IEquatable<TId>
{
    internal IEntityReference<TId> OwningReference { get; }
}
