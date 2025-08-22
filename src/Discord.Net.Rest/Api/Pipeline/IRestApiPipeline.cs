namespace Discord.Models.Rest.Api;

public interface IRestApiPipeline
{
    
}

public static class PipelineExtensions
{
    extension(IOperation operation)
    {
        public IRestApiPipeline CreatePipeline()
        {
            
        }
    }
}
