using Discord.Models;

namespace Discord.Rest;

public partial interface IRestNestedChannelTrait :
    IRestTraitProvider<RestGuildChannelActor>,
    INestedChannel
{
    [SourceOfTruth]
    new RestCategoryChannelActor? Category
    {
        get
        {
            var model = GetModel();

            if (model.ParentId.HasValue)
            {
                return GetOrCreateTraitData(
                    nameof(Category),
                    channel => channel.Guild.Channels.Category[model.ParentId.Value]
                );
            }
            
            ClearTraitData(nameof(Category));

            return null;
        }
    }
}