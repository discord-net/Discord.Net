using System;

namespace Discord
{
	internal abstract partial class DiscordWebSocket
	{
		//Debug
		public event EventHandler<LogMessageEventArgs> OnDebugMessage;
		protected void RaiseOnDebugMessage(DebugMessageType type, string message)
		{
			if (OnDebugMessage != null)
				OnDebugMessage(this, new LogMessageEventArgs(type, message));
		}

		//Connection
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
	}
}
