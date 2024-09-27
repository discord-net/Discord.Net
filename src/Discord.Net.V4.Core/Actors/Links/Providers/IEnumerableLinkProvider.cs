using MorseCode.ITask;

namespace Discord;

public interface IEnumerableLinkProvider<out TEntity>
    where TEntity : IEntity
{
    ITask<IReadOnlyCollection<TEntity>> AllAsync(RequestOptions? options = null, CancellationToken token = default);
}