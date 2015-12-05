using System;

namespace Discord
{
	public enum LogMessageSeverity : byte
	{
		Error = 1,
		Warning = 2,
		Info = 3,
		Verbose = 4,
		Debug = 5
	}
    public enum LogMessageSource : byte
	{
		Unknown = 0,
		Cache,
		Client,
		DataWebSocket,
        MessageQueue,
		Rest,
        VoiceWebSocket,
	}

	public class DisconnectedEventArgs : EventArgs
	{
		public readonly bool WasUnexpected;
		public readonly Exception Error;

		public DisconnectedEventArgs(bool wasUnexpected, Exception error) 
		{ 
			WasUnexpected = wasUnexpected; 
			Error = error; 
		}
	}
	public sealed class LogMessageEventArgs : EventArgs
	{
		public LogMessageSeverity Severity { get; }
		public LogMessageSource Source { get; }
		public string Message { get; }
		public Exception Exception { get; }

		public LogMessageEventArgs(LogMessageSeverity severity, LogMessageSource source, string msg, Exception exception)
		{ 
			Severity = severity; 
			Source = source; 
			Message = msg;
			Exception = exception;
        }
	}

	public sealed class VoicePacketEventArgs
	{
		public long UserId { get; }
		public long ChannelId { get; }
		public byte[] Buffer { get; }
		public int Offset { get; }
		public int Count { get; }

		public VoicePacketEventArgs(long userId, long channelId, byte[] buffer, int offset, int count)
		{
			UserId = userId;
			Buffer = buffer;
			Offset = offset;
			Count = count;
        }
	}

	public partial class DiscordWSClient
	{
		public event EventHandler Connected;
		private void RaiseConnected()
		{
			if (Connected != null)
				RaiseEvent(nameof(Connected), () => Connected(this, EventArgs.Empty));
		}
		public event EventHandler<DisconnectedEventArgs> Disconnected;
		private void RaiseDisconnected(DisconnectedEventArgs e)
		{
			if (Disconnected != null)
				RaiseEvent(nameof(Disconnected), () => Disconnected(this, e));
		}
		public event EventHandler<LogMessageEventArgs> LogMessage;
		protected void RaiseOnLog(LogMessageSeverity severity, LogMessageSource source, string message, Exception exception = null)
		{
			if (LogMessage != null)
				RaiseEvent(nameof(LogMessage), () => LogMessage(this, new LogMessageEventArgs(severity, source, message, exception)));
		}
	}
}
