using Discord.API;

namespace Discord.Tests.Framework.Mocks.Rest
{
    public static class Users
    {
        public static User SelfUser => new User()
        {
            Id = 103559217914318848,
            Username = "Jake",
            Discriminator = "0001",
            Email = "SelfUser@mocks.foxbot.me",
            MfaEnabled = true,
            Verified = true,
            Avatar = "cdd7ae679ef37ce03e097221c70aeed6"
        };

        public static User BotSelfUser => new User()
        {
            Id = 145584102551060480,
            Username = "foxboat",
            Discriminator = "8888",
            Email = null,
            MfaEnabled = true,
            Verified = true,
            Avatar = "5f0be46cea584d2225e6c15e8418fb52",
            Bot = true
        };
    }
}
