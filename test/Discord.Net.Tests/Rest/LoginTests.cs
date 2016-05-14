using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Rest;

namespace Discord.Tests.Rest
{
    [TestClass]
    public class LoginTests
    {
        public static TestContext Context;
        private static DiscordClient _client;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            Context = context;
            _client = new DiscordClient(new DiscordConfig() { RestClientProvider = (url, ct) => new TestRestClient(url, ct) });
            if (EndpointHandler.Instance == null) EndpointHandler.Instance = new EndpointHandler();
            if (Json.Serializer == null) new Json();
        }

        [TestMethod]
        [TestCategory("Login")]
        public async Task Test_Login_As_User()
        {
            Responses.Users.UserHandlers.Mode = Rest.Responses.Users.TestMode.User;
            await _client.Login(TokenType.User, "UserToken_Voltana");
        }
        [TestMethod]
        [ExpectedException(typeof(Net.HttpException))]
        [TestCategory("Login")]
        public async Task Test_Login_As_User_With_Invalid_Token()
        {
            Responses.Users.UserHandlers.Mode = Rest.Responses.Users.TestMode.User;
            await _client.Login(TokenType.User, "UserToken-NotVoltana");
        }
        [TestMethod]
        [TestCategory("Login")]
        public async Task Test_Login_As_Bot()
        {
            Responses.Users.UserHandlers.Mode = Rest.Responses.Users.TestMode.Bot;
            await _client.Login(TokenType.Bot, "UserToken_VoltanaBot");
        }
        [TestMethod]
        [ExpectedException(typeof(Net.HttpException))]
        [TestCategory("Login")]
        public async Task Test_Login_As_Bot_With_Invalid_Token()
        {
            Responses.Users.UserHandlers.Mode = Rest.Responses.Users.TestMode.Bot;
            await _client.Login(TokenType.Bot, "UserToken-NotVoltanaBot");
        }
    }
}
