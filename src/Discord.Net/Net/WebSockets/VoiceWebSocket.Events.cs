using System;

namespace Discord.Net.WebSockets
{
	internal sealed class IsTalkingEventArgs : EventArgs
	{
		public readonly string UserId;
		public readonly bool IsSpeaking;
		internal IsTalkingEventArgs(string userId, bool isTalking)
		{
			UserId = userId;
			IsSpeaking = isTalking;
		}
	}

	internal partial class VoiceWebSocket
	{
		public event EventHandler<IsTalkingEventArgs> IsSpeaking;
		private void RaiseIsSpeaking(string userId, bool isSpeaking)
		{
			if (IsSpeaking != null)
				IsSpeaking(this, new IsTalkingEventArgs(userId, isSpeaking));
		}

		public event EventHandler<VoicePacketEventArgs> OnPacket;
		internal void RaiseOnPacket(string userId, string channelId, byte[] buffer, int offset, int count)
		{
			if (OnPacket != null)
				OnPacket(this, new VoicePacketEventArgs(userId, channelId, buffer, offset, count));
		}
	}
}
