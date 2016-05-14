using Newtonsoft.Json;

namespace Discord.Tests.Rest.Responses.Users
{
    public class Mock_Me_User_Valid
    {
        [JsonProperty("id")]
        public string Id => "66078337084162048";
        [JsonProperty("username")]
        public string Username => "Voltana";
        [JsonProperty("discriminator")]
        public ushort Discriminator => 0001;
        [JsonProperty("avatar")]
        public string Avatar => "ec2b259bfe24686bf9d214b6bebe0834";
        [JsonProperty("verified")]
        public bool IsVerified => true;
        [JsonProperty("email")]
        public string Email => "hello-i-am-not-real@foxbot.me";
    }

    public class Mock_Me_Token_Valid
    {
        [JsonProperty("id")]
        public string Id => "66078337084162048";
        [JsonProperty("username")]
        public string Username => "foxboat";
        [JsonProperty("discriminator")]
        public ushort Discriminator => 0005;
        [JsonProperty("avatar")]
        public string Avatar => "ec2b259bfe24686bf9d214b6bebe0834";
        [JsonProperty("verified")]
        public bool IsVerified => true;
        [JsonProperty("email")]
        public string Email => "hello-i-am-not-real@foxbot.me";
        [JsonProperty("bot")]
        public bool Bot => true;
    }
}
