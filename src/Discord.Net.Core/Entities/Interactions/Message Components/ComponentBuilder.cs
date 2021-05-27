using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public class ComponentBuilder
    {
        public const int MaxActionRowCount = 5;

        public List<ActionRowBuilder> ActionRows
        {
            get => _actionRows;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(paramName: nameof(ActionRows), message: "Cannot set an component builder's components collection to null.");
                if (value.Count > MaxActionRowCount)
                    throw new ArgumentException(message: $"Action row count must be less than or equal to {MaxActionRowCount}.", paramName: nameof(ActionRows));
                _actionRows = value;
            }
        }

        private List<ActionRowBuilder> _actionRows { get; set; }

        public ComponentBuilder WithButton(
            string label,
            ButtonStyle style = ButtonStyle.Primary,
            IEmote emote = null,
            string customId = null,
            string url = null,
            bool disabled = false,
            int row = 0)
        {
            var button = new ButtonBuilder()
                .WithLabel(label)
                .WithStyle(style)
                .WithEmote(emote)
                .WithCustomId(customId)
                .WithUrl(url)
                .WithDisabled(disabled);

            return this.WithButton(button, row);
        }

        public ComponentBuilder WithButton(ButtonBuilder button)
            => this.WithButton(button, 0);

        public ComponentBuilder WithButton(ButtonBuilder button, int row)
        {
            var builtButton = button.Build();

            if (_actionRows == null)
            {
                _actionRows = new List<ActionRowBuilder>();
                _actionRows.Add(new ActionRowBuilder().WithComponent(builtButton));
            }
            else
            {
                if (_actionRows.Count + 1 == row)
                    _actionRows.Add(new ActionRowBuilder().WithComponent(builtButton));
                else
                {
                    if (_actionRows.Count > row)
                        _actionRows[row].WithComponent(builtButton);
                    else
                    {
                        _actionRows.First().WithComponent(builtButton);
                    }
                }
            }

            return this;
        }

        public MessageComponent Build()
        {
            if (this._actionRows != null)
                return new MessageComponent(this._actionRows.Select(x => x.Build()).ToList());
            else
                return MessageComponent.Empty;
        }
    }

    public class ActionRowBuilder
    {
        public const int MaxChildCount = 5;
        public List<IMessageComponent> Components
        {
            get => _components;
            set
            {
                if (value != null)
                    if (value.Count > MaxChildCount)
                        throw new ArgumentException(message: $"Action row can only contain {MaxChildCount} child components!", paramName: nameof(Components));
                _components = value;
            }
        }

        private List<IMessageComponent> _components { get; set; }

        public ActionRowBuilder WithComponents(List<IMessageComponent> components)
        {
            this.Components = components;
            return this;
        }

        public ActionRowBuilder WithComponent(IMessageComponent component)
        {
            if (this.Components == null)
                this.Components = new List<IMessageComponent>();

            this.Components.Add(component);

            return this;
        }

        public ActionRowComponent Build()
            => new ActionRowComponent(this._components);
    }

    public class ButtonBuilder
    {
        public const int MaxLabelLength = 80;
        public const int MaxCustomIdLength = 100;

        public string Label
        {
            get => _label;
            set
            {
                if (value != null)
                    if (value.Length > MaxLabelLength)
                        throw new ArgumentException(message: $"Button label must be {MaxLabelLength} characters or less!", paramName: nameof(Label));

                _label = value;
            }
        }

        public string CustomId
        {
            get => _customId;
            set
            {
                if (value != null)
                    if (value.Length > MaxCustomIdLength)
                        throw new ArgumentException(message: $"Custom Id must be {MaxCustomIdLength} characters or less!", paramName: nameof(CustomId));
                _customId = value;
            }
        }

        public ButtonStyle Style { get; set; }
        public IEmote Emote { get; set; }
        public string Url { get; set; }
        public bool Disabled { get; set; }


        private string _label;
        private string _customId;

        public static ButtonBuilder CreateLinkButton(string label, string url)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Link)
                .WithUrl(url)
                .WithLabel(label);

            return builder;
        }

        public static ButtonBuilder CreateDangerButton(string label, string customId)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Danger)
                .WithCustomId(customId)
                .WithLabel(label);

            return builder;
        }

        public static ButtonBuilder CreatePrimaryButton(string label, string customId)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Primary)
                .WithCustomId(customId)
                .WithLabel(label);

            return builder;
        }

        public static ButtonBuilder CreateSecondaryButton(string label, string customId)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Secondary)
                .WithCustomId(customId)
                .WithLabel(label);

            return builder;
        }

        public static ButtonBuilder CreateSuccessButton(string label, string customId)
        {
            var builder = new ButtonBuilder()
                .WithStyle(ButtonStyle.Success)
                .WithCustomId(customId)
                .WithLabel(label);

            return builder;
        }

        public ButtonBuilder WithLabel(string label)
        {
            this.Label = label;
            return this;
        }

        public ButtonBuilder WithStyle(ButtonStyle style)
        {
            this.Style = style;
            return this;
        }

        public ButtonBuilder WithEmote(IEmote emote)
        {
            this.Emote = emote;
            return this;
        }

        public ButtonBuilder WithUrl(string url)
        {
            this.Url = url;
            return this;
        }

        public ButtonBuilder WithCustomId(string id)
        {
            this.CustomId = id;
            return this;
        }
        public ButtonBuilder WithDisabled(bool disabled)
        {
            this.Disabled = disabled;
            return this;
        }

        public ButtonComponent Build()
        {
            if (string.IsNullOrEmpty(this.Label) && this.Emote == null)
                throw new ArgumentException("A button must have an Emote or a label!");

            if (!string.IsNullOrEmpty(this.Url) && !string.IsNullOrEmpty(this.CustomId))
                throw new InvalidOperationException("A button cannot contain a URL and a CustomId");

            if (this.Style == ButtonStyle.Link && !string.IsNullOrEmpty(this.CustomId))
                this.CustomId = null;
            else if (!string.IsNullOrEmpty(this.Url))
                this.Url = null;

            return new ButtonComponent(this.Style, this.Label, this.Emote, this.CustomId, this.Url, this.Disabled);
        }
    }
}
