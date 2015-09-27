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

		internal DisconnectedEventArgs(bool wasUnexpected, Exception error) 
		{ 
			WasUnexpected = wasUnexpected; 
			Error = error; 
		}
	}
	public class VoiceDisconnectedEventArgs : DisconnectedEventArgs
	{
		public readonly string ServerId;

		internal VoiceDisconnectedEventArgs(string serverId, DisconnectedEventArgs e)
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

		internal LogMessageEventArgs(LogMessageSeverity severity, LogMessageSource source, string msg) 
		{ 
			Severity = severity; 
			Source = source; 
			Message = msg; 
		}
	}

	public sealed class VoicePacketEventArgs
	{
		public string UserId { get; }
		public string ChannelId { get; }
		public byte[] Buffer { get; }
		public int Offset { get; }
		public int Count { get; }

		internal VoicePacketEventArgs(string userId, string channelId, byte[] buffer, int offset, int count)
		{
			UserId = userId;
			Buffer = buffer;
			Offset = offset;
			Count = count;
        }
	}

	public partial class DiscordSimpleClient
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
		internal void RaiseOnLog(LogMessageSeverity severity, LogMessageSource source, string message)
		{
			if (LogMessage != null)
				RaiseEvent(nameof(LogMessage), () => LogMessage(this, new LogMessageEventArgs(severity, source, message)));
		}

		public event EventHandler VoiceConnected;
		private void RaiseVoiceConnected()
		{
			if (VoiceConnected != null)
				RaiseEvent(nameof(VoiceConnected), () => VoiceConnected(this, EventArgs.Empty));
		}
		public event EventHandler<VoiceDisconnectedEventArgs> VoiceDisconnected;
		private void RaiseVoiceDisconnected(string serverId, DisconnectedEventArgs e)
		{
			if (VoiceDisconnected != null)
				RaiseEvent(nameof(VoiceDisconnected), () => VoiceDisconnected(this, new VoiceDisconnectedEventArgs(serverId, e)));
		}

		public event EventHandler<VoicePacketEventArgs> OnVoicePacket;
		internal void RaiseOnVoicePacket(VoicePacketEventArgs e)
		{
			if (OnVoicePacket != null)
				OnVoicePacket(this, e);
		}
	}
}
