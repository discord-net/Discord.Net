using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.State
{
    internal interface IEntityProvider<TId, TEntity>
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>
    {
        Optional<TId> Id { get; }

        bool TryGetReferenced([NotNullWhen(true)] out TEntity? entity);

        ValueTask<TEntity?> GetAsync(CancellationToken token = default);
        ValueTask<IEntityHandle<TId, TEntity>?> GetHandleAsync(CancellationToken token = default);
    }
}
