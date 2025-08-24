// using Discord.Models;
// using Discord.Rest.Api;
//
// namespace Discord.Rest;
//
// public interface IRestModifiable<out TOperation, in TProperties> :
//     IModifiable<TProperties>,
//     IOperationProvider<TOperation>
//     where TOperation : IOperation
// {
// }
//
// public interface IRestModifiable<
//     out TOperation,
//     in TProperties,
//     TEntity,
//     TCoreEntity
// > :
//     IModifiable<TProperties, TCoreEntity>,
//     IOperationProvider<TOperation>,
//     IRestClientProvider
//     where TOperation : IOperation
//     where TEntity : IRestEntity, IRestPipelineEntity<TEntity>, TCoreEntity
//     where TProperties : IParametersModel
//     where TCoreEntity : IEntity
// {
//     Task IModifiable<TProperties>.ModifyAsync(TProperties properties, RequestOptions options)
//         => RestModifiableExtensions.ModifyAsync(this, properties, options);
//
//     async Task<TCoreEntity> IModifiable<TProperties, TCoreEntity>.ModifyAsync(TProperties properties,
//         RequestOptions options)
//         => await RestModifiableExtensions.ModifyAsync(this, properties, options);
// }
//
// public static class RestModifiableExtensions
// {
//     extension<
//         TOperation,
//         TProperties,
//         TEntity,
//         TCoreEntity
//     >(
//         IRestModifiable<
//             TOperation,
//             TProperties,
//             TEntity,
//             TCoreEntity
//         > self
//     )
//         where TOperation : IOperation
//         where TEntity : IRestEntity, IRestPipelineEntity<TEntity>, TCoreEntity
//         where TProperties : IParametersModel
//         where TCoreEntity : IEntity
//     {
//         public async Task<TEntity> ModifyAsync(TProperties properties, RequestOptions options)
//             => await TEntity
//                 .FromPipeline(self.GetOperation().AsPipeline(body: new RequestBody.Json(properties)))
//                 .RunAsync(self.Client, options);
//     }
// }