namespace Discord.Models;

public interface IMessageComponentWithCustomId : IMessageComponentModel 
{
    string CustomId { get; }   
}