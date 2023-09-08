namespace Discord;

public interface IRestApiClient
{
    Task<T> GetAsync<T>(string route, BucketInfo bucket, RequestOptions options, CancellationToken token);

    Task<T> DeleteAsync<T>(string route, BucketInfo bucket, RequestOptions options, CancellationToken token);

    Task<T> PostAsync<T>(string route, object payload, BucketInfo bucket, RequestOptions options, CancellationToken token);

    Task<T> PatchAsync<T>(string route, object payload, BucketInfo bucket, RequestOptions options, CancellationToken token);

    Task<T> PutAsync<T>(string route, object payload, BucketInfo bucket, RequestOptions options, CancellationToken token);
}
