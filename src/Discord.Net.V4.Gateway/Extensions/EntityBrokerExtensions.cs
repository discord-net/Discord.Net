using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

internal static class EntityBrokerExtensions
{
    public static IManageableEntityBroker<TId, TTargetEntity, TTargetModel> Cast<
        TId, TSourceEntity, TSourceModel, TTargetEntity, TTargetModel
    >(this IManageableEntityBroker<TId, TSourceEntity, TSourceModel> broker)
        where TId : IEquatable<TId>
        where TSourceEntity : class, ICacheableEntity<TId>, IEntityOf<TSourceModel>
        where TSourceModel : IEntityModel<TId>
        where TTargetEntity : class, ICacheableEntity<TId>, IEntityOf<TTargetModel>
        where TTargetModel : IEntityModel<TId>
    {
        return new CastedEntityBroker<TId, TSourceEntity, TTargetEntity, TSourceModel, TTargetModel>(broker);
    }
}