using Discord.Interactions;
using Discord.WebSocket;
using InteractionFramework.Attributes;
using System.Threading.Tasks;

namespace InteractionFramework
{
    // As with all other modules, we create the context by defining what type of interaction this module is supposed to target.
    internal class ComponentModule : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
    {
        // With the Attribute DoUserCheck you can make sure that only the user this button targets can click it. This is defined by the first wildcard: *.
        // See Attributes/DoUserCheckAttribute.cs for elaboration.
        [DoUserCheck]
        [ComponentInteraction("myButton:*")]
        public async Task ClickButtonAsync(string userId)
            => await RespondAsync(text: ":thumbsup: Clicked!");
    }
}
