using Discord.Audio;
using System;

namespace Discord.Net.WebSockets
{
	internal sealed class IsTalkingEventArgs : EventArgs
	{
		public readonly ulong UserId;
		public readonly bool IsSpeaking;
		internal IsTalkingEventArgs(ulong userId, bool isTalking)
		{
			UserId = userId;
			IsSpeaking = isTalking;
		}
	}

	public partial class VoiceWebSocket
	{
		internal event EventHandler<IsTalkingEventArgs> IsSpeaking;
		private void RaiseIsSpeaking(ulong userId, bool isSpeaking)
		{
			if (IsSpeaking != null)
				IsSpeaking(this, new IsTalkingEventArgs(userId, isSpeaking));
		}

		internal event EventHandler<VoicePacketEventArgs> OnPacket;
		internal void RaiseOnPacket(ulong userId, ulong channelId, byte[] buffer, int offset, int count)
		{
			if (OnPacket != null)
				OnPacket(this, new VoicePacketEventArgs(userId, channelId, buffer, offset, count));
		}
	}
}
