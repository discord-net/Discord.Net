using System;
using System.Threading.Tasks;

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

		internal DisconnectedEventArgs(bool wasUnexpected, Exception error) 
		{ 
			WasUnexpected = wasUnexpected; 
			Error = error; 
		}
	}
	public class VoiceConnectedEventArgs : EventArgs
	{
		public readonly long ServerId;

		internal VoiceConnectedEventArgs(long serverId)
		{
			ServerId = serverId;
		}
	}
	public class VoiceDisconnectedEventArgs : DisconnectedEventArgs
	{
		public readonly long ServerId;

		internal VoiceDisconnectedEventArgs(long serverId, DisconnectedEventArgs e)
			: base(e.WasUnexpected, e.Error)
		{
			ServerId = serverId;
		}
	}
	public sealed class LogMessageEventArgs : EventArgs
	{
		public LogMessageSeverity Severity { get; }
		public LogMessageSource Source { get; }
		public string Message { get; }
		public Exception Exception { get; }

		internal LogMessageEventArgs(LogMessageSeverity severity, LogMessageSource source, string msg, Exception exception)
		{ 
			Severity = severity; 
			Source = source; 
			Message = msg;
			Exception = exception;
        }
	}

	public sealed class VoicePacketEventArgs : EventArgs
	{
		public long UserId { get; }
		public long ChannelId { get; }
		public byte[] Buffer { get; }
		public int Offset { get; }
		public int Count { get; }

		internal VoicePacketEventArgs(long userId, long channelId, byte[] buffer, int offset, int count)
		{
			UserId = userId;
			Buffer = buffer;
			Offset = offset;
			Count = count;
        }
	}

	public partial class DiscordWSClient
	{
		public event AsyncEventHandler<EventArgs> Connected { add { _connected.Add(value); } remove { _connected.Remove(value); } }
		private readonly AsyncEvent<EventArgs> _connected = new AsyncEvent<EventArgs>(nameof(Connected));
		protected Task RaiseConnected()
			=> RaiseEvent(_connected, EventArgs.Empty);

		public event AsyncEventHandler<DisconnectedEventArgs> Disconnected { add { _disconnected.Add(value); } remove { _disconnected.Remove(value); } }
		private readonly AsyncEvent<DisconnectedEventArgs> _disconnected = new AsyncEvent<DisconnectedEventArgs>(nameof(Disconnected));
		protected Task RaiseDisconnected(DisconnectedEventArgs e)
			=> RaiseEvent(_disconnected, e);

		public event AsyncEventHandler<VoiceConnectedEventArgs> VoiceConnected { add { _voiceConnected.Add(value); } remove { _voiceConnected.Remove(value); } }
		private readonly AsyncEvent<VoiceConnectedEventArgs> _voiceConnected = new AsyncEvent<VoiceConnectedEventArgs>(nameof(VoiceConnected));
		protected Task RaiseVoiceConnected(long serverId)
			=> RaiseEvent(_voiceConnected, new VoiceConnectedEventArgs(serverId));

		public event AsyncEventHandler<VoiceDisconnectedEventArgs> VoiceDisconnected { add { _voiceDisconnected.Add(value); } remove { _voiceDisconnected.Remove(value); } }
		private readonly AsyncEvent<VoiceDisconnectedEventArgs> _voiceDisconnected = new AsyncEvent<VoiceDisconnectedEventArgs>(nameof(VoiceDisconnected));
		protected Task RaiseVoiceDisconnected(long serverId, DisconnectedEventArgs e)
			=> RaiseEvent(_voiceDisconnected, new VoiceDisconnectedEventArgs(serverId, e));

		public event AsyncEventHandler<LogMessageEventArgs> LogMessage { add { _logMessage.Add(value); } remove { _logMessage.Remove(value); } }
		private readonly AsyncEvent<LogMessageEventArgs> _logMessage = new AsyncEvent<LogMessageEventArgs>(nameof(LogMessage));
		protected Task RaiseLogMessage(LogMessageSeverity severity, LogMessageSource source, string message, Exception exception = null)
			=> RaiseEvent(_logMessage, new LogMessageEventArgs(severity, source, message, exception));

		public event AsyncEventHandler<VoicePacketEventArgs> VoiceReceived { add { _voiceReceived.Add(value); } remove { _voiceReceived.Remove(value); } }
		private readonly AsyncEvent<VoicePacketEventArgs> _voiceReceived = new AsyncEvent<VoicePacketEventArgs>(nameof(VoiceReceived));
		protected Task RaiseVoiceReceived(long userId, long  channelId, byte[] buffer, int offset, int count)
			=> RaiseEvent(_voiceReceived, new VoicePacketEventArgs(userId, channelId, buffer, offset, count));
	}
}
