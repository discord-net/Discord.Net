using System;

namespace Discord.Net.WebSockets
{
	internal abstract partial class WebSocket
	{		
		public event EventHandler Connected;
		private void RaiseConnected()
		{
			if (Connected != null)
				Connected(this, EventArgs.Empty);
		}
		public event EventHandler<DisconnectedEventArgs> Disconnected;
		private void RaiseDisconnected(bool wasUnexpected, Exception error)
		{
			if (Disconnected != null)
				Disconnected(this, new DisconnectedEventArgs(wasUnexpected, error));
		}

		public event EventHandler<LogMessageEventArgs> LogMessage;
		internal void RaiseLogMessage(LogMessageSeverity severity, string message, Exception exception = null)
		{
			if (LogMessage != null)
				LogMessage(this, new LogMessageEventArgs(severity, LogMessageSource.Unknown, message, exception));
		}
	}
}
