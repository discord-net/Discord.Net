using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.Net.Tests
{
	[TestClass]
	public class Tests
	{
		private DiscordClient _bot1, _bot2;
		private Server _testServer;
		private Channel _testServerChannel;
		private Random _random;

		[TestInitialize]
		public void Initialize()
		{
			_random = new Random();

			_bot1 = new DiscordClient();
			_bot2 = new DiscordClient();

			_bot1.Connect(Settings.Test1_Username, Settings.Test1_Password).Wait();
			_bot2.Connect(Settings.Test2_Username, Settings.Test2_Password).Wait();

			//Cleanup existing servers
			Task.WaitAll(_bot1.Servers.Select(x => _bot1.LeaveServer(x)).ToArray());
			Task.WaitAll(_bot2.Servers.Select(x => _bot2.LeaveServer(x)).ToArray());

			_testServer = _bot1.CreateServer("Discord.Net Testbed", Regions.US_East).Result;
			_testServerChannel = _testServer.DefaultChannel;
			Invite invite = _bot1.CreateInvite(_testServer, 60, 1, false, false).Result;
			_bot2.AcceptInvite(invite).Wait();
		}

		[TestMethod]
		public void TestSendMessage()
		{
			string text = $"test_{_random.Next()}";
			AssertEvent<DiscordClient.MessageEventArgs>(
				"MessageCreated event never received",
				() => _bot1.SendMessage(_testServerChannel, text), 
				x => _bot2.MessageCreated += x, 
				x => _bot2.MessageCreated -= x,
				(s, e) => e.Message.Text == text);
        }

		[TestMethod]
		public void TestCreateTextRoom()
			=> TestCreateRoom(ChannelTypes.Text);
		[TestMethod]
		public void TestCreateVoiceRoom()
			=> TestCreateRoom(ChannelTypes.Voice);
		private void TestCreateRoom(string type)
		{
			Channel channel = null;
			string name = $"test_{_random.Next()}";
			AssertEvent<DiscordClient.ChannelEventArgs>(
				"ChannelCreated event never received",
				() => channel = _bot1.CreateChannel(_testServer, name, type).Result,
				x => _bot2.ChannelCreated += x,
				x => _bot2.ChannelCreated -= x,
				(s, e) => e.Channel.Name == name);

			AssertEvent<DiscordClient.ChannelEventArgs>(
				"ChannelDestroyed event never received",
				() => _bot1.DestroyChannel(channel),
				x => _bot2.ChannelDestroyed += x,
				x => _bot2.ChannelDestroyed -= x,
				(s, e) => e.Channel.Name == name);
		}

		[TestCleanup]
		public void Cleanup()
		{
			if (_bot1.IsConnected)
				Task.WaitAll(_bot1.Servers.Select(x => _bot1.LeaveServer(x)).ToArray());
			if (_bot2.IsConnected)
				Task.WaitAll(_bot2.Servers.Select(x => _bot2.LeaveServer(x)).ToArray());

			_bot1.Disconnect().Wait();
			_bot2.Disconnect().Wait();
		}

		private void AssertEvent<TArgs>(string msg, Action action, Action<EventHandler<TArgs>> addEvent, Action<EventHandler<TArgs>> removeEvent, Func<object, TArgs, bool> test = null)
		{
			ManualResetEvent trigger = new ManualResetEvent(false);
			bool result = false;

			EventHandler<TArgs> handler = (s, e) =>
			{
				if (test != null)
					result |= test(s, e);
				else
					result = true;
			};

			addEvent(handler);
			action();
			trigger.WaitOne(5000);
			removeEvent(handler);

			Assert.AreEqual(true, result, msg);
		}
	}
}
