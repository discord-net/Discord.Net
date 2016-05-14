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
    public class UserTests
    {
        public static TestContext Context;
        private static DiscordClient _client;

        [ClassInitialize]
        public static async Task Initialize(TestContext context)
        {
            Context = context;
            _client = new DiscordClient(new DiscordConfig() { RestClientProvider = (url, ct) => new TestRestClient(url, ct) });
            if (EndpointHandler.Instance == null) EndpointHandler.Instance = new EndpointHandler();
            Responses.Users.UserHandlers.Mode = Rest.Responses.Users.TestMode.User;
            await _client.Login(TokenType.User, "UserToken_Voltana");
        }
        
        [TestMethod]
        [TestCategory("Users")]
        public static async Task Test_Get_Current_User()
        {
            var currentUser = await _client.GetCurrentUser();
            Assert.AreEqual(66078337084162048, currentUser.Id, "Expected Id '66078337084162048'");
            Assert.AreEqual("Voltana", currentUser.Username, "Expected Name 'Voltana'");
            Assert.AreEqual(0001, currentUser.Discriminator, "Expected Discriminator '0001'");
            // Cannot Test Avatar URLs, Avatar ID not exposed publicly.
            Assert.AreEqual(true, currentUser.IsVerified, "Expected Verified 'true'");
            Assert.AreEqual("hello-i-am-not-real@foxbot.me", currentUser.Email, "Expected Email 'hello-i-am-not-real@foxbot.me'");
        }
    }
}
