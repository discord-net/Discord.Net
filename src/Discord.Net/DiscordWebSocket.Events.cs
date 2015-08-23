using System;

namespace Discord
{
	internal abstract partial class DiscordWebSocket
	{
		public event EventHandler Connected;
		private void RaiseConnected()
		{
			if (Connected != null)
				Connected(this, EventArgs.Empty);
		}

		public event EventHandler Disconnected;
		private void RaiseDisconnected()
		{
			if (Disconnected != null)
				Disconnected(this, EventArgs.Empty);
		}

		public event EventHandler<DiscordClient.LogMessageEventArgs> OnDebugMessage;
		protected void RaiseOnDebugMessage(string message)
		{
			if (OnDebugMessage != null)
				OnDebugMessage(this, new DiscordClient.LogMessageEventArgs(message));
		}
	}
}
