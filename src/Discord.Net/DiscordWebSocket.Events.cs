using System;

namespace Discord
{
	public class DisconnectedEventArgs : EventArgs
	{
		public readonly bool WasUnexpected;
		internal DisconnectedEventArgs(bool wasUnexpected) { WasUnexpected = wasUnexpected; }
	}

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
		public event EventHandler<DisconnectedEventArgs> Disconnected;
		private void RaiseDisconnected(bool wasUnexpected)
		{
			if (Disconnected != null)
				Disconnected(this, new DisconnectedEventArgs(wasUnexpected));
		}
	}
}
