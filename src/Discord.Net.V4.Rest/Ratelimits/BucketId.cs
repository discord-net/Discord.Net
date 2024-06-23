namespace Discord;

internal record struct BucketId(string Endpoint, string? Hash = null, BucketInfo? Info = null)
{
    public bool Represents(IApiRoute route)
    {
        if (route.Bucket is null)
            return Endpoint == route.Endpoint;

        return Info is not null && Info.Value.Equals(route.Bucket);
    }
}
