using Discord.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Net.Tests
{
	[TestClass]
	public class ChannelTests
	{
		private DiscordClient _bot1, _bot2;

		[TestInitialize]
		public void Initialize()
		{
			_bot1 = new DiscordClient();
			_bot2 = new DiscordClient();

			_bot1.Connect(Settings.Test1_Username, Settings.Test1_Password).Wait();
			_bot2.Connect(Settings.Test2_Username, Settings.Test2_Password).Wait();

			//Cleanup existing servers
			Task.WaitAll(_bot1.Servers.Select(x => _bot1.LeaveServer(x)).ToArray());
			Task.WaitAll(_bot2.Servers.Select(x => _bot2.LeaveServer(x)).ToArray());
		}
		
		[TestMethod]
		public async Task DoNothing()
		{
			Server server = await _bot1.CreateServer("Discord.Net Testbed", Region.US_East);
			Invite invite = await _bot1.CreateInvite(server, 60, 1, false, false);
			await _bot2.AcceptInvite(invite);
			await _bot2.LeaveServer(server);
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
	}
}
