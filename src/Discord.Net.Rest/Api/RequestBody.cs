using Discord.Models.Models;

namespace Discord.Models.Rest.Api;

public abstract record RequestBody
{
    public sealed record Json(IParametersModel model) : RequestBody;
    public sealed record Multipart(IMultipartParametersModel model) : RequestBody;
    
}