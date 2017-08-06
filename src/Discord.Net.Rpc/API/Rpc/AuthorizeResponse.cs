#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class AuthorizeResponse
    {
        [ModelProperty("code")]
        public string Code { get; set; }
    }
}
