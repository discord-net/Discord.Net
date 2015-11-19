using System;

namespace Discord.Net.WebSockets
{
	internal sealed class IsTalkingEventArgs : EventArgs
	{
		public readonly long UserId;
		public readonly bool IsSpeaking;
		internal IsTalkingEventArgs(long userId, bool isTalking)
		{
			UserId = userId;
			IsSpeaking = isTalking;
		}
	}

	internal partial class VoiceWebSocket
	{
		public event EventHandler<IsTalkingEventArgs> IsSpeaking;
		private void RaiseIsSpeaking(long userId, bool isSpeaking)
		{
			if (IsSpeaking != null)
				IsSpeaking(this, new IsTalkingEventArgs(userId, isSpeaking));
		}

		public event EventHandler<VoicePacketEventArgs> OnPacket;
		internal void RaiseOnPacket(long userId, long channelId, byte[] buffer, int offset, int count)
		{
			if (OnPacket != null)
				OnPacket(this, new VoicePacketEventArgs(userId, channelId, buffer, offset, count));
		}
	}
}
