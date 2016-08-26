using Newtonsoft.Json;
using Discord.API;

namespace Discord.Tests.Framework.Responses.Users
{
    public static class User_Mocks
    {
        public static User Me_User
        {
            get
            {
                return new User
                {
                    Id = 66078337084162048,
                    Username = "Voltana",
                    Discriminator = "0001",
                    Avatar = "ec2b259bfe24686bf9d214b6bebe0834",
                    Verified = true,
                    Email = "hello-i-am-not-real@foxbot.me"
                };
            }
        }

        public static User Bot_User
        {
            get
            {
                return new User
                {
                    Id = 66078337084162048,
                    Username = "foxboat",
                    Discriminator = "0005",
                    Avatar = "ec2b259bfe24686bf9d214b6bebe0834",
                    Verified = true,
                    Email = "hello-i-am-not-real@foxbot.me",
                    Bot = true
                };
            }
        }

        public static User Public_User
        {
            get
            {
                return new User
                {
                    Id = 96642168176807936,
                    Username = "Khionu",
                    Discriminator = "9999",
                    Avatar = "ceeff590f1e0e1ccae0afc89967131ff"
                };
            }
        }
    }
}
