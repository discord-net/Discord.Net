using System.Collections.Immutable;
using System.Linq;

namespace Discord.Rest;

internal static class MessageComponentExtension
{
    internal static IMessageComponent ToModel(this IMessageComponent component)
    {
        switch (component)
        {
            case ActionRowComponent actionRow:
                return new API.ActionRowComponent(actionRow);

            case ButtonComponent btn:
                return new API.ButtonComponent(btn);

            case SelectMenuComponent select:
                return new API.SelectMenuComponent(select);

            case TextInputComponent textInput:
                return new API.TextInputComponent(textInput);

            case TextDisplayComponent textDisplay:
                return new API.TextDisplayComponent(textDisplay);

            case SectionComponent section:
                return new API.SectionComponent(section);

            case ThumbnailComponent thumbnail:
                return new API.ThumbnailComponent(thumbnail);

            case MediaGalleryComponent mediaGallery:
                return new API.MediaGalleryComponent(mediaGallery);

            case SeparatorComponent separator:
                return new API.SeparatorComponent(separator);

            case FileComponent file:
                return new API.FileComponent(file);

            case ContainerComponent container:
                return new API.ContainerComponent(container);
        }

        return null;
    }

    internal static IMessageComponent ToEntity(this IMessageComponent component)
    {
        switch (component.Type)
        {
            case ComponentType.ActionRow:
            {
                var parsed = (API.ActionRowComponent)component;
                return new ActionRowComponent
                {
                    Id = component.Id,
                    Components = parsed.Components.Select(x => x.ToEntity()).ToImmutableArray()
                };
            }

            case ComponentType.Button:
            {
                var parsed = (API.ButtonComponent)component;
                return new ButtonComponent(
                    parsed.Style,
                    parsed.Label.GetValueOrDefault(),
                    parsed.Emote.IsSpecified
                        ? parsed.Emote.Value.Id.HasValue
                            ? new Emote(parsed.Emote.Value.Id.Value, parsed.Emote.Value.Name, parsed.Emote.Value.Animated.GetValueOrDefault())
                            : new Emoji(parsed.Emote.Value.Name)
                        : null,
                    parsed.CustomId.GetValueOrDefault(),
                    parsed.Url.GetValueOrDefault(),
                    parsed.Disabled.GetValueOrDefault(),
                    parsed.SkuId.ToNullable(),
                    parsed.Id.ToNullable());
            }

            case ComponentType.SelectMenu or ComponentType.ChannelSelect or ComponentType.RoleSelect or ComponentType.MentionableSelect or ComponentType.UserSelect:
            {
                var parsed = (API.SelectMenuComponent)component;
                return new SelectMenuComponent(
                    parsed.CustomId,
                    parsed.Options?.Select(z => new SelectMenuOption(
                        z.Label,
                        z.Value,
                        z.Description.GetValueOrDefault(),
                        z.Emoji.IsSpecified
                            ? z.Emoji.Value.Id.HasValue
                                ? new Emote(z.Emoji.Value.Id.Value, z.Emoji.Value.Name, z.Emoji.Value.Animated.GetValueOrDefault())
                                : new Emoji(z.Emoji.Value.Name)
                            : null,
                        z.Default.ToNullable())).ToList(),
                    parsed.Placeholder.GetValueOrDefault(),
                    parsed.MinValues,
                    parsed.MaxValues,
                    parsed.Disabled,
                    parsed.Type,
                    parsed.Id.ToNullable(),
                    parsed.ChannelTypes.GetValueOrDefault(),
                    parsed.DefaultValues.IsSpecified
                        ? parsed.DefaultValues.Value.Select(x => new SelectMenuDefaultValue(x.Id, x.Type))
                        : []
                );
            }

            case ComponentType.TextInput:
            {
                var parsed = (API.TextInputComponent)component;
                return new TextInputComponent(parsed.CustomId,
                    parsed.Label,
                    parsed.Placeholder.GetValueOrDefault(null),
                    parsed.MinLength.ToNullable(),
                    parsed.MaxLength.ToNullable(),
                    parsed.Style,
                    parsed.Required.ToNullable(),
                    parsed.Value.GetValueOrDefault(null),
                    parsed.Id.ToNullable());
            }

            case ComponentType.TextDisplay:
            {
                var parsed = (API.TextDisplayComponent)component;
                return new TextDisplayComponent(parsed.Content, parsed.Id.ToNullable());
            }

            case ComponentType.Section:
            {
                var parsed = (API.SectionComponent)component;
                return new SectionComponent(parsed.Id.ToNullable(),
                    parsed.Components.Select(x => x.ToEntity()).ToImmutableArray(),
                    parsed.Accessory.ToEntity());
            }

            case ComponentType.Thumbnail:
            {
                var parsed = (API.ThumbnailComponent)component;
                return new ThumbnailComponent(parsed.Id.ToNullable(),
                    parsed.Media.ToEntity(),
                    parsed.Description.GetValueOrDefault(null),
                    parsed.IsSpoiler.ToNullable());
            }

            case ComponentType.MediaGallery:
            {
                var parsed = (API.MediaGalleryComponent)component;

                return new MediaGalleryComponent(
                    parsed.Items.Select(x => new MediaGalleryItem(x.Media.ToEntity(), x.Description.GetValueOrDefault(null), x.IsSpoiler.GetValueOrDefault(false))).ToList(),
                    parsed.Id.ToNullable());
            }

            case ComponentType.Separator:
            {
                var parsed = (API.SeparatorComponent)component;
                return new SeparatorComponent(parsed.IsDivider.ToNullable(), parsed.Spacing.ToNullable(), parsed.Id.ToNullable());
            }

            case ComponentType.File:
            {
                var parsed = (API.FileComponent)component;
                return new FileComponent(parsed.File.ToEntity(), parsed.IsSpoiler.ToNullable(), parsed.Id.ToNullable());
            }

            case ComponentType.Container:
            {
                var parsed = (API.ContainerComponent)component;
                return new ContainerComponent(parsed.Components.Select(x => x.ToEntity()).ToImmutableArray(),
                    parsed.AccentColor.GetValueOrDefault(null),
                    parsed.IsSpoiler.ToNullable(),
                    parsed.Id.ToNullable());
            }

            default:
                return null;
        }
    }

    internal static UnfurledMediaItem ToEntity(this API.UnfurledMediaItem mediaItem)
    {
        return new ResolvedUnfurledMediaItem(mediaItem.Url,
            mediaItem.ProxyUrl.GetValueOrDefault(null),
            mediaItem.Height.GetValueOrDefault(0).GetValueOrDefault(0),
            mediaItem.Width.GetValueOrDefault(0).GetValueOrDefault(0),
            mediaItem.ContentType.GetValueOrDefault(null),
            mediaItem.LoadingState.GetValueOrDefault(UnfurledMediaItemLoadingState.Unknown));
    }

    internal static API.UnfurledMediaItem ToModel(this UnfurledMediaItem mediaItem)
    {
        return new API.UnfurledMediaItem
        {
            Url = mediaItem.Url
        };
    }
}
