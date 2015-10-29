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
        public bool IsHidden { get; internal set; }
        public string Description { get; internal set; }
		internal Func<CommandEventArgs, Task> Handler;

		internal Command(string text)
		{
			Text = text;
            IsHidden = false; // Set false by default to avoid null error
            Description = "No description set for this command.";
		}
	}
}
