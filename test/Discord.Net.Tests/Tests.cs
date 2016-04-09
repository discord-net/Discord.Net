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

        private static DiscordSocketClient _hostBot, _targetBot, _observerBot;
        private static Guild _testGuild;
        private static TextChannel _testGuildChannel;
        private static Random _random;
        private static PublicInvite _testGuildInvite;

        private static TestContext _context;

        private static string _hostToken;
        private static string _observerToken;
        private static string _targetToken;

        private static string GetRandomText()
        {
            lock (_random)
                return $"test_{_random.Next()}";
        }

        #region Initialization

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _context = testContext;

            _hostToken = Environment.GetEnvironmentVariable("discord-unit-host_token");
            _observerToken = Environment.GetEnvironmentVariable("discord-unit-observer_token");
            _targetToken = Environment.GetEnvironmentVariable("discord-unit-target_token");
        }

        [TestMethod]
        [Priority(1)]
        public async Task TestInitialize()
        {
            _context.WriteLine("Initializing.");

            _random = new Random();

            _hostBot = new DiscordSocketClient(_hostToken);
            _targetBot = new DiscordSocketClient(_targetToken);
            _observerBot = new DiscordSocketClient(_observerToken);

            await _hostBot.Login();

            await Task.Delay(3000);

            //Cleanup existing Guilds
            (await _hostBot.GetGuilds()).Select(x => x.Owner.Id == _hostBot.CurrentUser.Id ? x.Delete() : x.Leave());

            //Create new Guild and invite the other bots to it

            _testGuild = await _hostBot.CreateGuild("Discord.Net Testing", _hostBot.GetOptimalVoiceRegion());

            await Task.Delay(1000);

            PublicInvite invite = await _testGuild.CreateInvite(60, 3, false, false);
            _testGuildInvite = invite;

            _context.WriteLine($"Host: {_hostBot.CurrentUser.Username} in {(await _hostBot.GetGuilds()).Count()}");
        }

        [TestMethod]
        [Priority(2)]
        public async Task TestTokenLogin_Ready()
        {
            AssertEvent(
                "READY never received",
                async () => await _observerBot.Login(),
                x => _observerBot.Connected += x,
                x => _observerBot.Connected -= x,
                null,
                true);
            (await _observerBot.GetGuilds()).Select(x => x.Owner.Id == _observerBot.CurrentUser.Id ? x.Delete() : x.Leave());
            await _observerBot.RestClient.Send(new API.Rest.AcceptInviteRequest(_testGuildInvite.Code));
        }

        [TestMethod]
        [Priority(2)]
        public async Task TestReady()
        {
            AssertEvent(
                "READY never received",
                async () => await _targetBot.Login(),
                x => _targetBot.Connected += x,
                x => _targetBot.Connected -= x,
                null,
                true);

            (await _targetBot.GetGuilds()).Select(x => x.Owner.Id == _targetBot.CurrentUser.Id ? x.Delete() : x.Leave());
            _testGuildChannel = _testGuild.DefaultChannel;
        }

        #endregion

        // Guilds

        #region Guild Tests

        [TestMethod]
        [Priority(3)]
        public void TestJoinedGuild()
        {
            AssertEvent<GuildEventArgs>(
                "Never Got JoinedGuild",
                async () => await _targetBot.RestClient.Send(new API.Rest.AcceptInviteRequest(_testGuildInvite.Code)),
                x => _targetBot.JoinedGuild += x,
                x => _targetBot.JoinedGuild -= x);
        }

        #endregion

        #region Channel Tests

        //Channels
        [TestMethod]
        public void TestCreateTextChannel()
        {
            GuildChannel channel = null;
            string name = GetRandomText();
            AssertEvent<ChannelEventArgs>(
                "ChannelCreated event never received",
                async () => channel = await _testGuild.CreateTextChannel(name),
                x => _targetBot.ChannelCreated += x,
                x => _targetBot.ChannelCreated -= x,
                (s, e) => e.Channel.Id == channel.Id);

            AssertEvent<ChannelEventArgs>(
                "ChannelDestroyed event never received",
                async () => await channel.Delete(),
                x => _targetBot.ChannelDestroyed += x,
                x => _targetBot.ChannelDestroyed -= x,
                (s, e) => e.Channel.Id == channel.Id);
        }
        [TestMethod]
        public void TestCreateVoiceChannel()
        {
            GuildChannel channel = null;
            string name = GetRandomText();
            AssertEvent<ChannelEventArgs>(
                "ChannelCreated event never received",
                async () => channel = await _testGuild.CreateVoiceChannel(name),
                x => _targetBot.ChannelCreated += x,
                x => _targetBot.ChannelCreated -= x,
                (s, e) => e.Channel.Id == channel.Id);

            AssertEvent<ChannelEventArgs>(
                "ChannelDestroyed event never received",
                async () => await channel.Delete(),
                x => _targetBot.ChannelDestroyed += x,
                x => _targetBot.ChannelDestroyed -= x,
                (s, e) => e.Channel.Id == channel.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(Net.HttpException))]
        public async Task TestCreateChannel_NoName()
        {
            await _testGuild.CreateTextChannel($"");
        }
        [TestMethod]
        public async Task Test_CreateGetChannel()
        {
            var name = GetRandomText();
            var channel = await _testGuild.CreateTextChannel(name);
            var get_channel = _testGuild.GetChannel(channel.Id);
            Assert.AreEqual(channel.Id, get_channel.Id, "ID of Channel and GetChannel were not equal.");
        }
        [TestMethod]
        public void TestSendTyping()
        {
            var channel = _testGuildChannel;
            AssertEvent<TypingEventArgs>(
                "UserUpdated event never fired.",
                async () => await channel.TriggerTyping(),
                x => _targetBot.UserIsTyping += x,
                x => _targetBot.UserIsTyping -= x);
        }
        [TestMethod]
        public void TestEditChannel()
        {
            var channel = _testGuildChannel;
            AssertEvent<ChannelUpdatedEventArgs>(
                "ChannelUpdated Never Received",
                async () => await channel.Modify(x =>
                {
                    x.Name = GetRandomText();
                    x.Topic = $"topic - {GetRandomText()}";
                    x.Position = 26;
                }),
                x => _targetBot.ChannelUpdated += x,
                x => _targetBot.ChannelUpdated -= x);
        }
        [TestMethod]
        public void TestChannelMention()
        {
            var channel = _testGuildChannel;
            Assert.AreEqual($"<#{channel.Id}>", channel.Mention, "Generated channel mention was not the expected channel mention.");
        }
        [TestMethod]
        public void TestChannelUserCount()
        {
            Assert.AreEqual(3, _testGuildChannel.Users.Count(), "Read an incorrect number of users in a channel");
        }

        #endregion

        #region Message Tests

        //Messages
        [TestMethod]
        public async Task TestMessageEvents()
        {
            string name = GetRandomText();
            var channel = await _testGuild.CreateTextChannel(name);
            _context.WriteLine($"Channel Name: {channel.Name} / {channel.Guild.Name}");
            string text = GetRandomText();
            Message message = null;
            AssertEvent<MessageEventArgs>(
                "MessageCreated event never received",
                async () => message = await channel.SendMessage(text),
                x => _targetBot.MessageReceived += x,
                x => _targetBot.MessageReceived -= x,
                (s, e) => e.Message.Text == text);

            AssertEvent<MessageUpdatedEventArgs>(
                "MessageUpdated event never received",
                async () => await message.Modify(x =>
                {
                    x.Content = text + " updated";
                }),
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
        public async Task TestDownloadMessages()
        {
            string name = GetRandomText();
            var channel = await _testGuild.CreateTextChannel(name);
            for (var i = 0; i < 10; i++) await channel.SendMessage(GetRandomText());
            while (channel.Discord.MessageQueue.Count > 0) await Task.Delay(100);
            var messages = await channel.GetMessages(10);
            Assert.AreEqual(10, messages.Count(), "Expected 10 messages in downloaded array, did not see 10.");
        }
        [TestMethod]
        public async Task TestSendTTSMessage()
        {
            var channel = await _testGuild.CreateTextChannel(GetRandomText());
            AssertEvent<MessageEventArgs>(
                "MessageCreated event never fired",
                async () => await channel.SendMessage(GetRandomText(), true),
                x => _targetBot.MessageReceived += x,
                x => _targetBot.MessageReceived -= x,
                (s, e) => e.Message.IsTTS);
        }

        #endregion

        #region User Tests

        [TestMethod]
        public async Task TestUserMentions()
        {
            var user = (await _targetBot.GetGuild(_testGuild.Id)).CurrentUser;
            Assert.AreEqual($"<@{user.Id}>", user.Mention);
        }
        [TestMethod]
        public void TestUserEdit()
        {
            var user = _testGuild.GetUser(_targetBot.CurrentUser.Id);
            AssertEvent<UserUpdatedEventArgs>(
                "UserUpdated never fired",
                async () => await user.Modify(x =>
                {
                    x.Deaf = true;
                    x.Mute = true;
                }),
                x => _targetBot.UserUpdated += x,
                x => _targetBot.UserUpdated -= x);
        }
        [TestMethod]
        public void TestEditSelf()
        {
            throw new NotImplementedException();
            /*var name = RandomText
            AssertEvent<UserUpdatedEventArgs>(
                "UserUpdated never fired",
                async () => await _targetBot.CurrentUser.Modify(TargetPassword, name),
                x => _obGuildBot.UserUpdated += x,
                x => _obGuildBot.UserUpdated -= x,
                (s, e) => e.After.Username == name);*/
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
        private Task SetStatus(DiscordClient _client, UserStatus status)
        {
            throw new NotImplementedException();
            /*_client.SetStatus(status);
            await Task.Delay(50);*/
        }
        [TestMethod]
        public void TestSetGame()
        {
            AssertEvent<UserUpdatedEventArgs>(
                "UserUpdated never fired",
                async () => await SetGame(_targetBot, "test game"),
                x => _observerBot.UserUpdated += x,
                x => _observerBot.UserUpdated -= x,
                (s, e) => _targetBot.CurrentUser.CurrentGame == "test game");

        }
        private Task SetGame(DiscordClient _client, string game)
        {
            throw new NotImplementedException();
            //_client.SetGame(game);
            //await Task.Delay(5);
        }

        #endregion

        #region Permission Tests

        // Permissions
        [TestMethod]
        public async Task Test_AddGet_PermissionsRule()
        {
            var channel = await _testGuild.CreateTextChannel(GetRandomText());
            var user = _testGuild.GetUser(_targetBot.CurrentUser.Id);
            var perms = new OverwritePermissions(sendMessages: PermValue.Deny);
            await channel.UpdatePermissionOverwrite(user, perms);
            var resultPerms = channel.GetPermissionOverwrite(user);
            Assert.IsNotNull(resultPerms, "Perms retrieved from Guild were null.");
        }
        [TestMethod]
        public async Task Test_AddRemove_PermissionsRule()
        {
            var channel = await _testGuild.CreateTextChannel(GetRandomText());
            var user = _testGuild.GetUser(_targetBot.CurrentUser.Id);
            var perms = new OverwritePermissions(sendMessages: PermValue.Deny);
            await channel.UpdatePermissionOverwrite(user, perms);
            await channel.RemovePermissionOverwrite(user);
            await Task.Delay(200);
            Assert.AreEqual(PermValue.Inherit, channel.GetPermissionOverwrite(user)?.SendMessages);
        }
        [TestMethod]
        public async Task Test_Permissions_Event()
        {
            var channel = await _testGuild.CreateTextChannel(GetRandomText());
            var user = _testGuild.GetUser(_targetBot.CurrentUser.Id);
            var perms = new OverwritePermissions(sendMessages: PermValue.Deny);
            AssertEvent<ChannelUpdatedEventArgs>
                ("ChannelUpdatedEvent never fired.",
                async () => await channel.UpdatePermissionOverwrite(user, perms),
                x => _targetBot.ChannelUpdated += x,
                x => _targetBot.ChannelUpdated -= x,
                (s, e) => e.Channel == channel && (e.After as GuildChannel).PermissionOverwrites.Count() != (e.Before as GuildChannel).PermissionOverwrites.Count());
        }
        [TestMethod]
        [ExpectedException(typeof(Net.HttpException))]
        public async Task Test_Affect_Permissions_Invalid_Channel()
        {
            var channel = await _testGuild.CreateTextChannel(GetRandomText());
            var user = _testGuild.GetUser(_targetBot.CurrentUser.Id);
            var perms = new OverwritePermissions(sendMessages: PermValue.Deny);
            await channel.Delete();
            await channel.UpdatePermissionOverwrite(user, perms);
        }

        #endregion


        [ClassCleanup]
        public static async Task Cleanup()
        {
            WaitMany(
                (await _hostBot.GetGuilds()).Select(x => x.Owner.Id == _hostBot.CurrentUser.Id ? x.Delete() : x.Leave()),
                (await _targetBot.GetGuilds()).Select(x => x.Owner.Id == _targetBot.CurrentUser.Id ? x.Delete() : x.Leave()),
                (await _observerBot.GetGuilds()).Select(x => x.Owner.Id == _observerBot.CurrentUser.Id ? x.Delete() : x.Leave()));

            WaitAll(
                _hostBot.Disconnect(),
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
