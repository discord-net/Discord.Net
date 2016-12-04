namespace Discord
{
    public class ModifyTextChannelParams : ModifyGuildChannelParams
    {
        public Optional<string> Topic { get; set; }
    }
}
