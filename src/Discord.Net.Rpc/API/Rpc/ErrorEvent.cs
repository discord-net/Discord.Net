#pragma warning disable CS1591
using Discord.Serialization;

namespace Discord.API.Rpc
{
    internal class ErrorEvent
    {
        [ModelProperty("code")]
        public int Code { get; set; }
        [ModelProperty("message")]
        public string Message { get; set; }
    }
}
