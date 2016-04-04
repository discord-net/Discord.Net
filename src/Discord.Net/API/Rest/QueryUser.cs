using Discord.Net.Rest;
using System;

namespace Discord.API.Rest
{
    public class QueryUserRequest : IRestRequest<User[]>
    {
        string IRestRequest.Method => "GET";
        string IRestRequest.Endpoint => $"users?q={Uri.EscapeDataString(Query)}&limit={Limit}";
        object IRestRequest.Payload => null;
        
        public string Query { get; set; }
        public int Limit { get; set; } = 25;

        public QueryUserRequest()
        {
        }
    }
}
