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
        private const int EventTimeout = 5000; //Max time in milliseconds to wait for an event response from our test actions

        private static DiscordClient _hostClient, _targetBot, _observerBot;
        private static Server _testServer;
        private static Channel _testServerChannel;
        private static Random _random;
        private static Invite _testServerInvite;

        private static TestContext _context;

        [ClassInitialize]
        public async static void Initialize(TestContext testContext)
        {
            _context = testContext;

            /*_context.WriteLine("Initializing.");

            var settings = Settings.Instance;
            _random = new Random();

            _hostClient = new DiscordClient();
            _targetBot = new DiscordClient();
            _observerBot = new DiscordClient();

            await _hostClient.Connect(settings.User1.Email, settings.User1.Password);
            await _targetBot.Connect(settings.User2.Email, settings.User2.Password);
            await _observerBot.Connect(settings.User3.Email, settings.User3.Password);

            await Task.Delay(1000);

            //Cleanup existing servers
            WaitMany(
                _hostClient.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave()),
                _targetBot.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave()),
                _observerBot.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave()));
            //Create new server and invite the other bots to it

            _testServer = await _hostClient.CreateServer("Discord.Net Testing", _hostClient.Regions.First());
            _testServerChannel = _testServer.DefaultChannel;

            await Task.Delay(1000);

            Invite invite = await _testServer.CreateInvite(60, 3, false, false);
            WaitAll(
                (await _targetBot.GetInvite(invite.Code)).Accept(),
                (await _observerBot.GetInvite(invite.Code)).Accept());*/
        }

        [TestMethod]
        [Priority(1)]
        public async Task TestInitialize()
        {
            _context.WriteLine("Initializing.");

            var settings = Settings.Instance;
            _random = new Random();

            _hostClient = new DiscordClient();
            _targetBot = new DiscordClient();
            _observerBot = new DiscordClient();

            await _hostClient.Connect(settings.User1.Email, settings.User1.Password);
            await _observerBot.Connect(settings.User3.Email, settings.User3.Password);

            await Task.Delay(3000);

            //Cleanup existing servers
            WaitMany(
                _hostClient.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave()),
                _observerBot.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave()));
            //Create new server and invite the other bots to it

            _testServer = await _hostClient.CreateServer("Discord.Net Testing", _hostClient.Regions.First());
            _testServerChannel = _testServer.DefaultChannel;

            await Task.Delay(1000);

            Invite invite = await _testServer.CreateInvite(60, 3, false, false);
            _testServerInvite = invite;

            await (await _observerBot.GetInvite(invite.Code)).Accept();

            await Task.Delay(1000);

            _context.WriteLine($"Host: {_hostClient.CurrentUser.Name} in {_hostClient.Servers.Count()}");
            _context.WriteLine($"Observer: {_observerBot.CurrentUser.Name} in {_observerBot.Servers.Count()}");
        }

        // READY
        [TestMethod]
        [Priority(2)]
        public void TestReady()
        {
            AssertEvent(
                "READY never received",
                async () => await _targetBot.Connect(Settings.Instance.User2.Email, Settings.Instance.User2.Password),
                x => _targetBot.Ready += x,
                x => _targetBot.Ready -= x,
                null,
                true);

            _targetBot.Servers.Select(x => x.IsOwner ? x.Delete() : x.Leave());
        }

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

        //Messages
        [TestMethod]
        public void TestMessageReceived()
        {
            string text = $"test_{_random.Next()}";
            AssertEvent<MessageEventArgs>(
                "MessageCreated event never received",
                () => _testServerChannel.SendMessage(text),
                x => _targetBot.MessageReceived += x,
                x => _targetBot.MessageReceived -= x);
        }

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
    }
}
