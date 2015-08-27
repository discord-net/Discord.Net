using System;

namespace Discord.Helpers
{
	internal partial class JsonHttpClient
	{
		public event EventHandler<LogMessageEventArgs> OnDebugMessage;
		protected void RaiseOnDebugMessage(DebugMessageType type, string message)
		{
			if (OnDebugMessage != null)
				OnDebugMessage(this, new LogMessageEventArgs(type, message));
		}
	}
}
