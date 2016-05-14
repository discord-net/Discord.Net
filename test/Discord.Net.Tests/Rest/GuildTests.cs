using Discord.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Tests.Rest
{
    [TestClass]
    public class GuildTests
    {
        public static TestContext Context;
        private static IDiscordClient _client;

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
        [TestCategory("Guilds")]
        public async Task Test_Get_Guild()
        {
            var guild = await _client.GetGuild(66078535390867456);
            Assert.AreEqual(66078535390867456UL, guild.Id, "Expected ID '66078535390867456'");
            Assert.AreEqual("Discord API", guild.Name, "Expected Name 'Discord API'");
            // Cannot Verify Guild URL, ID not publicly exposed.
            Assert.IsNull(guild.SplashUrl, "Expected SplashUrl 'null'");
            Assert.AreEqual(66078337084162048UL, guild.OwnerId, "Expected OwnerId '66078337084162048'");
            Assert.AreEqual(3600, guild.AFKTimeout, "Expected AFKTimeout '3600'");
            Assert.AreEqual(true, guild.IsEmbeddable, "Expected Embeddable 'true'");
            Assert.IsNull(guild.EmbedChannelId, "Expected EmbedChannelId 'null'");
            Assert.AreEqual(0, guild.VerificationLevel, "Expected VerificationLevel '0'");
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Get_Guild_Invalid_Id()
        {
            var guild = await _client.GetGuild(1);
            Assert.IsNull(guild);
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Get_Guilds()
        {
            var guilds = await _client.GetGuilds();
            Assert.AreEqual(2, guilds.Count(), "Expected 2 Guilds");
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Guild_Get_Bans()
        {
            var guild = await _client.GetGuild(66078535390867456);
            var bans = await guild.GetBans();
            Assert.AreEqual(2, bans.Count());
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Guild_Get_User()
        {
            var guild = await _client.GetGuild(66078535390867456);
            var user = await guild.GetUser(66078337084162048);
            // TODO: Asserts
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Guild_Get_Invalid_User()
        {
            var guild = await _client.GetGuild(66078535390867456);
            var user = await guild.GetUser(1);
            Assert.IsNull(user, "Expected returned user to be null");
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Guild_Get_Users()
        {
            var guild = await _client.GetGuild(66078535390867456);
            var users = await guild.GetUsers();
            Assert.AreEqual(2, users.Count());
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Guild_Get_Role()
        {
            var guild = await _client.GetGuild(66078535390867456);
            var role = await guild.GetRole(1);
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Guild_Get_Invalid_Role()
        {
            var guild = await _client.GetGuild(66078535390867456);
            var role = await guild.GetRole(1);
            Assert.IsNull(role, "Expected returned role to be null.");
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Guild_Get_Roles()
        {
            var guild = await _client.GetGuild(66078535390867456);
            var roles = await guild.GetRoles();
        }
        [TestMethod]
        [TestCategory("Guilds")]
        public async Task Test_Guild_Get_Invites()
        {
            var guild = await _client.GetGuild(66078535390867456);
            var invites = await guild.GetInvites();
        }
    }
}
