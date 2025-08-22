namespace Discord.Models.Rest.Api;

public enum AuthenticationScheme
{
    None = 0,
    
    BotToken = 1 << 0,
    BearerToken = 1 << 1
}