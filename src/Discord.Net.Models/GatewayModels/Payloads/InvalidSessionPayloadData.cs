namespace Discord.Models.Json;

public sealed class InvalidSessionPayloadData : IInvalidSessionPayloadData
{
    public bool CanResume { get; set; }
}
