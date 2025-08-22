using Discord.Models;
using Discord.Models.Models;

namespace Discord.Rest.Api;

public abstract record RequestBody
{
    public sealed record Json(IParametersModel model) : RequestBody;
    public sealed record Multipart(IMultipartParametersModel model) : RequestBody;
    
}