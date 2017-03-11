using System;
using System.Threading.Tasks;

namespace MyBot
{
	public class Program
	{
		public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

		public async Task MainAsync()
		{
		}
	}
}