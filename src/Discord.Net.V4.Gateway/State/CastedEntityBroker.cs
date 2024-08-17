using System.Diagnostics.CodeAnalysis;
using Discord.Models;

namespace Discord.Gateway.State;

internal sealed class CastedEntityBroker<
    TId,
    TSourceEntity,
    TTargetEntity,
    TSourceModel,
    TTargetModel
>(IManageableEntityBroker<TId, TSourceEntity, TSourceModel> source) :
    IManageableEntityBroker<TId, TTargetEntity, TTargetModel>
    where TId : IEquatable<TId>
    where TSourceEntity : class, ICacheableEntity<TId>, IEntityOf<TSourceModel>
    where TSourceModel : IEntityModel<TId>
    where TTargetEntity : class, ICacheableEntity<TId>, IEntityOf<TTargetModel>
    where TTargetModel : IEntityModel<TId>
{
    private readonly IManageableEntityBroker<TId, TSourceEntity, TSourceModel> _source = source;

    public ValueTask<IEntityHandle<TId>> TransferConstructionOfEntityAsync(
        TTargetModel model,
        IGatewayConstructionContext context,
        CancellationToken token)
    {
        if (model is not TSourceModel source)
            throw new InvalidCastException(
                $"Expected a model of type '{typeof(TSourceModel)}', but got {model?.GetType() ?? typeof(TTargetModel)}"
            );

        return _source.TransferConstructionOfEntityAsync(source, context, token);
    }

    public ValueTask UpdateInReferenceEntityAsync(TTargetModel model, CancellationToken token)
    {
        if (model is not TSourceModel source)
            throw new InvalidCastException(
                $"Expected a model of type '{typeof(TSourceModel)}', but got {model?.GetType() ?? typeof(TTargetModel)}"
            );

        return _source.UpdateInReferenceEntityAsync(source, token);
    }

    public ValueTask UpdateInReferenceEntitiesAsync(IEnumerable<TTargetModel> models, CancellationToken token)
    {
        return _source.UpdateInReferenceEntitiesAsync(
            models.Cast<TSourceModel>(),
            token
        );
    }

    public bool TryGetHandle(TId id, [MaybeNullWhen(false)] out IEntityHandle<TId> handle)
        => _source.TryGetHandle(id, out handle);

    public ValueTask<IDisposable> GetEntityLockHandleAsync(TId id, CancellationToken token = default)
        => _source.GetEntityLockHandleAsync(id, token);
}