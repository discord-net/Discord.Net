namespace Discord
{
    public class UserIsSpeakingEventArgs : UserEventArgs
    {
        public bool IsSpeaking { get; }

        public UserIsSpeakingEventArgs(User user, bool isSpeaking)
            : base(user)
        {
            IsSpeaking = isSpeaking;
        }
    }
}
