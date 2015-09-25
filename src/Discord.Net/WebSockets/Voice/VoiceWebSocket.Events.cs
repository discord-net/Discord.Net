using System;

namespace Discord.WebSockets.Voice
{
	public sealed class IsTalkingEventArgs : EventArgs
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
	}
}
