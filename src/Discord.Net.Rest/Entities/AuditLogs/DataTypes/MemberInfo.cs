namespace Discord.Rest
{
    public struct MemberInfo
    {
        internal MemberInfo(string nick, bool? deaf, bool? mute)
        {
            Nickname = nick;
            Deaf = deaf;
            Mute = mute;
        }

        public string Nickname { get; }
        public bool? Deaf { get; }
        public bool? Mute { get; }
    }
}
