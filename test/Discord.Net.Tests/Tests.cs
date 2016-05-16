using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Tests
{
    //TODO: Tests are massively incomplete and out of date, needing a full rewrite

    [TestClass]
    public class Tests
    {
        private const int EventTimeout = 10000; //Max time in milliseconds to wait for an event response from our test actions

        private static DiscordClient _hostClient, _targetBot, _observerBot;
        private static Server _testServer;
        private static Channel _testServerChannel;
        private static Random _random;
        private static Invite _testServerInvite;

        private static TestContext _context;

        private static string HostBotToken;
        private static string ObserverBotToken;
        private static string TargetEmail;
        private static string TargetPassword;

        public static string RandomText => $"test_{_random.Next()}";

        #region Initialization

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _context = testContext;

            HostBotToken = Environment.GetEnvironmentVariable("discord-unit-host_token");
            ObserverBotToken = Environment.GetEnvironmentVariable("discord-unit-observer_token");
            TargetEmail = Environment.GetEnvironmentVariable("discord-unit-target_email");
            TargetPassword = Environment.GetEnvironmentVariable("discord-unit-target_pass");
        }

        [TestMethod]
        [Priority(1)]
        public async Task TestInitialize()
        {
            _context.WriteLine("Initializing.");

            _random = new Random();

            _hostClient = new DiscordClient();
            _targetBot = new DiscordClient();
            _observerBot = new DiscordClient();

            await _hostClient.Connect(HostBotToken);

            await Task.Delay(3000);

            //Cleanup existing servers
            _hostClient.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave());
            
            //Create new server and invite the other bots to it

            _testServer = await _hostClient.CreateServer("Discord.Net Testing", _hostClient.Regions.First());

            await Task.Delay(1000);

            Invite invite = await _testServer.CreateInvite(60, 3, false, false);
            _testServerInvite = invite;

            _context.WriteLine($"Host: {_hostClient.CurrentUser.Name} in {_hostClient.Servers.Count()}");
        }

        [TestMethod]
        [Priority(2)]
        public async Task TestTokenLogin_Ready()
        {
            AssertEvent(
                "READY never received",
                async () => await _observerBot.Connect(ObserverBotToken),
                x => _observerBot.Ready += x,
                x => _observerBot.Ready -= x,
                null,
                true);
            _observerBot.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave());
            await (await _observerBot.GetInvite(_testServerInvite.Code)).Accept();
        }

        [TestMethod]
        [Priority(2)]
        public void TestReady()
        {
            AssertEvent(
                "READY never received",
                async () => await _targetBot.Connect(TargetEmail, TargetPassword),
                x => _targetBot.Ready += x,
                x => _targetBot.Ready -= x,
                null,
                true);

            _targetBot.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave());
            _testServerChannel = _testServer.DefaultChannel;
        }

        #endregion

        // Servers

        #region Server Tests

        [TestMethod]
        [Priority(3)]
        public void TestJoinedServer()
        {
            AssertEvent<ServerEventArgs>(
                "Never Got JoinedServer",
                async () => await (await _targetBot.GetInvite(_testServerInvite.Code)).Accept(),
                x => _targetBot.JoinedServer += x,
                x => _targetBot.JoinedServer -= x);
        }

        #endregion

        #region Channel Tests

        //Channels
        [TestMethod]
        public void TestCreateTextChannel()
            => TestCreateChannel(ChannelType.Text);
        [TestMethod]
        public void TestCreateVoiceChannel()
            => TestCreateChannel(ChannelType.Voice);
        private void TestCreateChannel(ChannelType type)
        {
            _context.WriteLine($"Host: {_hostClient.CurrentUser.Name} in {_hostClient.Servers.Count()}");
            _context.WriteLine($"Target: {_targetBot.CurrentUser.Name} in {_targetBot.Servers.Count()}");
            _context.WriteLine($"Observer: {_observerBot.CurrentUser.Name} in {_observerBot.Servers.Count()}");
            Channel channel = null;
            string name = $"test_{_random.Next()}";
            AssertEvent<ChannelEventArgs>(
                "ChannelCreated event never received",
                async () => channel = await _testServer.CreateChannel(name, type),
                x => _targetBot.ChannelCreated += x,
                x => _targetBot.ChannelCreated -= x,
                (s, e) => e.Channel.Name == name);

            AssertEvent<ChannelEventArgs>(
                "ChannelDestroyed event never received",
                async () => await channel.Delete(),
                x => _targetBot.ChannelDestroyed += x,
                x => _targetBot.ChannelDestroyed -= x,
                (s, e) => e.Channel.Name == name);
        }

        [TestMethod]
        [ExpectedException(typeof(Net.HttpException))]
        public async Task TestCreateChannel_NoName()
        {
            await _testServer.CreateChannel($"", ChannelType.Text);
        }
        [TestMethod]
        [ExpectedException(typeof(Net.HttpException))]
        public async Task TestCreateChannel_NoType()
        {
            string name = $"#test_{_random.Next()}";
            await _testServer.CreateChannel($"", ChannelType.FromString(""));
        }
        [TestMethod]
        [ExpectedException(typeof(Net.HttpException))]
        public async Task TestCreateChannel_BadType()
        {
            string name = $"#test_{_random.Next()}";
            await _testServer.CreateChannel($"", ChannelType.FromString("badtype"));
        }
        [TestMethod]
        public async Task Test_CreateGetChannel()
        {
            var name = $"test_{_random.Next()}";
            var channel = await _testServer.CreateChannel(name, ChannelType.Text);
            var get_channel = _testServer.GetChannel(channel.Id);
            Assert.AreEqual(channel.Id, get_channel.Id, "ID of Channel and GetChannel were not equal.");
        }
        [TestMethod]
        public void TestSendTyping()
        {
            var channel = _testServerChannel;
            AssertEvent<ChannelUserEventArgs>(
                "UserUpdated event never fired.",
                async () => await channel.SendIsTyping(),
                x => _targetBot.UserIsTyping += x,
                x => _targetBot.UserIsTyping -= x);
        }
        [TestMethod]
        public void TestEditChannel()
        {
            var channel = _testServerChannel;
            AssertEvent<ChannelUpdatedEventArgs>(
                "ChannelUpdated Never Received",
                async () => await channel.Edit(RandomText, $"topic - {RandomText}", 26),
                x => _targetBot.ChannelUpdated += x,
                x => _targetBot.ChannelUpdated -= x);
        }
        [TestMethod]
        public void TestChannelMention()
        {
            var channel = _testServerChannel;
            Assert.AreEqual($"<#{channel.Id}>", channel.Mention, "Generated channel mention was not the expected channel mention.");
        }
        [TestMethod]
        public void TestChannelUserCount()
        {
            Assert.AreEqual(3, _testServerChannel.Users.Count(), "Read an incorrect number of users in a channel");
        }

        #endregion

        #region Message Tests

        //Messages
        [TestMethod]
        public void TestMessageEvents()
        {
            string name = $"test_{_random.Next()}";
            var channel = _testServer.CreateChannel(name, ChannelType.Text).Result;
            _context.WriteLine($"Channel Name: {channel.Name} / {channel.Server.Name}");
            string text = $"test_{_random.Next()}";
            Message message = null;
            AssertEvent<MessageEventArgs>(
                "MessageCreated event never received",
                async () => message = await channel.SendMessage(text),
                x => _targetBot.MessageReceived += x,
                x => _targetBot.MessageReceived -= x,
                (s, e) => e.Message.Text == text);

            AssertEvent<MessageUpdatedEventArgs>(
                "MessageUpdated event never received",
                async () => await message.Edit(text + " updated"),
                x => _targetBot.MessageUpdated += x,
                x => _targetBot.MessageUpdated -= x,
                (s, e) => e.Before.Text == text && e.After.Text == text + " updated");

            AssertEvent<MessageEventArgs>(
                "MessageDeleted event never received",
                async () => await message.Delete(),
                x => _targetBot.MessageDeleted += x,
                x => _targetBot.MessageDeleted -= x,
                (s, e) => e.Message.Id == message.Id);
        }
        [TestMethod]
        public async Task TestDownloadMessages_WithCache()
        {
            string name = $"test_{_random.Next()}";
            var channel = await _testServer.CreateChannel(name, ChannelType.Text);
            for (var i = 0; i < 10; i++) await channel.SendMessage(RandomText);
            while (channel.Client.MessageQueue.Count > 0) await Task.Delay(100);
            var messages = await channel.DownloadMessages(10);
            Assert.AreEqual(10, messages.Count(), "Expected 10 messages in downloaded array, did not see 10.");
        }
        [TestMethod]
        public async Task TestDownloadMessages_WithoutCache()
        {
            string name = $"test_{_random.Next()}";
            var channel = await _testServer.CreateChannel(name, ChannelType.Text);
            for (var i = 0; i < 10; i++) await channel.SendMessage(RandomText);
            while (channel.Client.MessageQueue.Count > 0) await Task.Delay(100);
            var messages = await channel.DownloadMessages(10, useCache: false);
            Assert.AreEqual(10, messages.Count(), "Expected 10 messages in downloaded array, did not see 10.");
        }
        [TestMethod]
        public async Task TestSendTTSMessage()
        {
            var channel = await _testServer.CreateChannel(RandomText, ChannelType.Text);
            AssertEvent<MessageEventArgs>(
                "MessageCreated event never fired",
                async () => await channel.SendTTSMessage(RandomText),
                x => _targetBot.MessageReceived += x,
                x => _targetBot.MessageReceived -= x,
                (s, e) => e.Message.IsTTS);
        }

        #endregion

        #region User Tests

        [TestMethod]
        public void TestUserMentions()
        {
            var user = _targetBot.GetServer(_testServer.Id).CurrentUser;
            Assert.AreEqual($"<@{user.Id}>", user.Mention);
        }
        [TestMethod]
        public void TestUserEdit()
        {
            var user = _testServer.GetUser(_targetBot.CurrentUser.Id);
            AssertEvent<UserUpdatedEventArgs>(
                "UserUpdated never fired",
                async () => await user.Edit(true, true, null, null),
                x => _targetBot.UserUpdated += x,
                x => _targetBot.UserUpdated -= x);
        }
        [TestMethod]
        public void TestEditSelf()
        {
            var name = $"test_{_random.Next()}";
            AssertEvent<UserUpdatedEventArgs>(
                "UserUpdated never fired",
                async () => await _targetBot.CurrentUser.Edit(TargetPassword, name),
                x => _observerBot.UserUpdated += x,
                x => _observerBot.UserUpdated -= x,
                (s, e) => e.After.Name == name);
        }
        [TestMethod]
        public void TestSetStatus()
        {
            AssertEvent<UserUpdatedEventArgs>(
                "UserUpdated never fired",
                async () => await SetStatus(_targetBot, UserStatus.Idle),
                x => _observerBot.UserUpdated += x,
                x => _observerBot.UserUpdated -= x,
                (s, e) => e.After.Status == UserStatus.Idle);
        }
        private async Task SetStatus(DiscordClient _client, UserStatus status)
        {
            _client.SetStatus(status);
            await Task.Delay(50);
        }
        [TestMethod]
        public void TestSetGame()
        {
            AssertEvent<UserUpdatedEventArgs>(
                "UserUpdated never fired",
                async () => await SetGame(_targetBot, "test game"),
                x => _observerBot.UserUpdated += x,
                x => _observerBot.UserUpdated -= x,
                (s, e) => _targetBot.CurrentGame.Name == "test game");

        }
        private async Task SetGame(DiscordClient _client, string game)
        {
            _client.SetGame(game);
            await Task.Delay(5);
        }

        #endregion

        #region Permission Tests

        // Permissions
        [TestMethod]
        public async Task Test_AddGet_PermissionsRule()
        {
            var channel = await _testServer.CreateChannel($"test_{_random.Next()}", ChannelType.Text);
            var user = _testServer.GetUser(_targetBot.CurrentUser.Id);
            var perms = new ChannelPermissionOverrides(sendMessages: PermValue.Deny);
            await channel.AddPermissionsRule(user, perms);
            var resultPerms = channel.GetPermissionsRule(user);
            Assert.IsNotNull(resultPerms, "Perms retrieved from server were null.");
        }
        [TestMethod]
        public async Task Test_AddRemove_PermissionsRule()
        {
            var channel = await _testServer.CreateChannel($"test_{_random.Next()}", ChannelType.Text);
            var user = _testServer.GetUser(_targetBot.CurrentUser.Id);
            var perms = new ChannelPermissionOverrides(sendMessages: PermValue.Deny);
            await channel.AddPermissionsRule(user, perms);
            await channel.RemovePermissionsRule(user);
            await Task.Delay(200);
            Assert.AreEqual(PermValue.Inherit, channel.GetPermissionsRule(user).SendMessages);
        }
        [TestMethod]
        public async Task Test_Permissions_Event()
        {
            var channel = await _testServer.CreateChannel($"test_{_random.Next()}", ChannelType.Text);
            var user = _testServer.GetUser(_targetBot.CurrentUser.Id);
            var perms = new ChannelPermissionOverrides(sendMessages: PermValue.Deny);
            AssertEvent<ChannelUpdatedEventArgs>
                ("ChannelUpdatedEvent never fired.",
                async () => await channel.AddPermissionsRule(user, perms),
                x => _targetBot.ChannelUpdated += x,
                x => _targetBot.ChannelUpdated -= x,
                (s, e) => e.After.PermissionOverwrites.Count() != e.Before.PermissionOverwrites.Count());
        }
        [TestMethod]
        [ExpectedException(typeof(Net.HttpException))]
        public async Task Test_Affect_Permissions_Invalid_Channel()
        {
            var channel = await _testServer.CreateChannel($"test_{_random.Next()}", ChannelType.Text);
            var user = _testServer.GetUser(_targetBot.CurrentUser.Id);
            var perms = new ChannelPermissionOverrides(sendMessages: PermValue.Deny);
            await channel.Delete();
            await channel.AddPermissionsRule(user, perms);
        }

        #endregion


        [ClassCleanup]
        public static void Cleanup()
        {
            WaitMany(
                _hostClient.State == ConnectionState.Connected ? _hostClient.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave()) : null,
                _targetBot.State == ConnectionState.Connected ? _targetBot.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave()) : null,
                _observerBot.State == ConnectionState.Connected ? _observerBot.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave()) : null);

            WaitAll(
                _hostClient.Disconnect(),
                _targetBot.Disconnect(),
                _observerBot.Disconnect());
        }

        #region Helpers

        // Task Helpers

        private static void AssertEvent<TArgs>(string msg, Func<Task> action, Action<EventHandler<TArgs>> addEvent, Action<EventHandler<TArgs>> removeEvent, Func<object, TArgs, bool> test = null)
        {
            AssertEvent(msg, action, addEvent, removeEvent, test, true);
        }
        private static void AssertNoEvent<TArgs>(string msg, Func<Task> action, Action<EventHandler<TArgs>> addEvent, Action<EventHandler<TArgs>> removeEvent, Func<object, TArgs, bool> test = null)
        {
            AssertEvent(msg, action, addEvent, removeEvent, test, false);
        }
        private static void AssertEvent<TArgs>(string msg, Func<Task> action, Action<EventHandler<TArgs>> addEvent, Action<EventHandler<TArgs>> removeEvent, Func<object, TArgs, bool> test, bool assertTrue)
        {
            ManualResetEventSlim trigger = new ManualResetEventSlim(false);
            bool result = false;

            EventHandler<TArgs> handler = (s, e) =>
            {
                if (test != null)
                {
                    result |= test(s, e);
                    trigger.Set();
                }
                else
                    result = true;
            };

            addEvent(handler);
            var task = action();
            trigger.Wait(EventTimeout);
            task.Wait();
            removeEvent(handler);

            Assert.AreEqual(assertTrue, result, msg);
        }

        private static void AssertEvent(string msg, Func<Task> action, Action<EventHandler> addEvent, Action<EventHandler> removeEvent, Func<object, bool> test, bool assertTrue)
        {
            ManualResetEventSlim trigger = new ManualResetEventSlim(false);
            bool result = false;

            EventHandler handler = (s, e) =>
            {
                if (test != null)
                {
                    result |= test(s);
                    trigger.Set();
                }
                else
                    result = true;
            };

            addEvent(handler);
            var task = action();
            trigger.Wait(EventTimeout);
            task.Wait();
            removeEvent(handler);

            Assert.AreEqual(assertTrue, result, msg);
        }

        private static void WaitAll(params Task[] tasks)
        {
            Task.WaitAll(tasks);
        }
        private static void WaitAll(IEnumerable<Task> tasks)
        {
            Task.WaitAll(tasks.ToArray());
        }
        private static void WaitMany(params IEnumerable<Task>[] tasks)
        {
            Task.WaitAll(tasks.Where(x => x != null).SelectMany(x => x).ToArray());
        }

        #endregion
    }
}
