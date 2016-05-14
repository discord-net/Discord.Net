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
        public static void Initialize(TestContext context)
        {
            Context = context;
            _client = new DiscordClient(new DiscordConfig() { RestClientProvider = (url, ct) => new TestRestClient(url, ct) });
            if (EndpointHandler.Instance == null) EndpointHandler.Instance = new EndpointHandler();
            if (Json.Serializer == null) new Json();
            Responses.Users.UserHandlers.Mode = Rest.Responses.Users.TestMode.User;
            _client.Login(TokenType.User, "UserToken_Voltana").Wait();
        }
        
        [TestMethod]
        [TestCategory("Users")]
        public async Task Test_Get_Current_User()
        {
            var currentUser = await _client.GetCurrentUser();
            Assert.AreEqual((UInt64)66078337084162048, currentUser.Id, "Expected Id '66078337084162048'");
            Assert.AreEqual("Voltana", currentUser.Username, "Expected Name 'Voltana'");
            Assert.AreEqual(0001, currentUser.Discriminator, "Expected Discriminator '0001'");
            // Cannot Test Avatar URLs, Avatar ID not exposed publicly.
            Assert.AreEqual(true, currentUser.IsVerified, "Expected Verified 'true'");
            Assert.AreEqual("hello-i-am-not-real@foxbot.me", currentUser.Email, "Expected Email 'hello-i-am-not-real@foxbot.me'");
            Assert.AreEqual(UserStatus.Unknown, currentUser.Status, "UserStatus should not be populated.");
            Assert.AreEqual(false, currentUser.IsBot, "Expected IsBot 'false'");
            Assert.AreEqual("<@66078337084162048>", currentUser.Mention, "Expected Mention '<@66078337084162048>'");
            Assert.IsNull(currentUser.CurrentGame, "CurrentGame should not be populated.");
            Assert.AreEqual(new DateTime(635714215032370000), currentUser.CreatedAt, "Expected Created At '635714215032370000'");
        }
        [TestMethod]
        [TestCategory("Users")]
        public async Task Test_Get_User()
        {
            var user = await _client.GetUser(96642168176807936);
            Assert.AreEqual((UInt64)96642168176807936, user.Id, "Expected Id '96642168176807936'");
            Assert.AreEqual("Khionu", user.Username, "Expected Name 'Khionu'");
            Assert.AreEqual(9999, user.Discriminator, "Expected Discriminator '0001'");
            // Cannot Test Avatar URLs, Avatar ID not exposed publicly.
            Assert.AreEqual(false, user.IsBot, "Expected IsBot 'false'");
            Assert.AreEqual("<@!96642168176807936>", user.NicknameMention, "Expected Mention '<@!96642168176807936>'");
            Assert.AreEqual(new DateTime(635787084884180000), user.CreatedAt, "Expected Created At '635787084884180000'");
        }
        [TestMethod]
        [TestCategory("Users")]
        public async Task Test_Get_Invalid_User()
        {
            var user = await _client.GetUser(1);
            Assert.IsNull(user, "Expected Invalid User to be 'null'");
        }
    }
}
