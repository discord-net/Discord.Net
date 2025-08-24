using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMessageActivityModel : IModel
{
    MessageActivityType Type { get; }
    Optional<string> PartyId { get; }
}

public enum MessageActivityType
{
    Join = 1,
    Spectate = 2,
    Listen = 3,
    JoinRequest = 4
}