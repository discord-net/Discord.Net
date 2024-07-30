using Discord.Gateway.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway;

internal static class EntityHandleExtensions
{
    public static async ValueTask<TEntity> ConsumeAsync<TId, TEntity>(this IEntityHandle<TId, TEntity> handle)
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        var entity = handle.Entity;
        await handle.DisposeAsync();
        return entity;
    }
}
