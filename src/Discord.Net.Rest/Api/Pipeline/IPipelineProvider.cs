namespace Discord.Rest.Api;

public interface IPipelineProvider<TOut>
{
    IRestApiPipeline<TOut> GetPipeline();
}