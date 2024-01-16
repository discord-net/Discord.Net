namespace Discord.API.Rest
{
    internal class CreateGuildBanParams
    {
        public Optional<int> DeleteMessageDays { get; set; }
        public string Reason { get; set; }
    }
}
