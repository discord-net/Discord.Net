namespace Discord;

internal record struct BucketId(string Endpoint, string? Hash = null, BucketInfo? Info = null)
{
    public bool Represents(ApiRoute route)
    {
        if (route.Bucket is null)
            return Endpoint == route.Endpoint;

        if (Info is not null)
            return Info.Value.Equals(route.Bucket);

        return false;
    }
}
