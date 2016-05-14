using Newtonsoft.Json;

namespace Discord.Tests.Rest.Responses.Users
{
    class Mock_ID_PublicUser
    {
        [JsonProperty("id")]
        public string Id => "96642168176807936";
        [JsonProperty("username")]
        public string Username => "Khionu";
        [JsonProperty("discriminator")]
        public ushort Discriminator => 9999;
        [JsonProperty("avatar")]
        public string Avatar => "ceeff590f1e0e1ccae0afc89967131ff";
    }
}
