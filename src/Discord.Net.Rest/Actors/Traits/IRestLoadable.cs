// using Discord.Rest.Api;
//
// namespace Discord.Rest;
//
// public interface IRestLoadable<
//     out TOperation,
//     TEntity,
//     TCoreEntity
// > : 
//     IRestClientProvider,
//     ILoadable<TCoreEntity>,
//     IOperationProvider<TOperation>
//     where TEntity : TCoreEntity, IRestPipelineEntity<TEntity>
//     where TOperation : IOperation
// {
//     new ValueTask<TEntity> GetAsync(RequestOptions options = default)
//         => TEntity.FromPipeline(GetOperation().AsPipeline())
//             .RunAsync(Client, options);
//
//     async ValueTask<TCoreEntity> ILoadable<TCoreEntity>.GetAsync(RequestOptions options)
//         => await GetAsync(options);
// }