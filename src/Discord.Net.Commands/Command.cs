using System;
using System.Threading.Tasks;

namespace Discord.Commands
{
	public sealed class Command
	{
		public string Text { get; }
		public int? MinArgs { get; internal set; }
		public int? MaxArgs { get; internal set; }
		public int MinPerms { get; internal set; }
		internal readonly string[] Parts;
		internal Func<CommandEventArgs, Task> Handler;

		internal Command(string text)
		{
			Text = text;
			Parts = text.ToLowerInvariant().Split(' ');
		}
	}
}
