namespace Discord.Rest.Api;

public abstract record RouteParameter
{
    public sealed record ChannelId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(ChannelId self) => self.Value;
        public static implicit operator ChannelId(Snowflake value) => new(value);
    }
    public sealed record ApplicationId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(ApplicationId self) => self.Value;
        public static implicit operator ApplicationId(Snowflake value) => new(value);
    }
    public sealed record GuildId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(GuildId self) => self.Value;
        public static implicit operator GuildId(Snowflake value) => new(value);
    }
    public sealed record CommandId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(CommandId self) => self.Value;
        public static implicit operator CommandId(Snowflake value) => new(value);
    }
    public sealed record MessageId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(MessageId self) => self.Value;
        public static implicit operator MessageId(Snowflake value) => new(value);
    }
    public sealed record EmojiName(string Value) : RouteParameter
    {
        public static implicit operator string(EmojiName self) => self.Value;
        public static implicit operator EmojiName(string value) => new(value);
    }
    public sealed record EntitlementId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(EntitlementId self) => self.Value;
        public static implicit operator EntitlementId(Snowflake value) => new(value);
    }
    public sealed record UserId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(UserId self) => self.Value;
        public static implicit operator UserId(Snowflake value) => new(value);
    }
    public sealed record AnswerId(int Value) : RouteParameter
    {
        public static implicit operator int(AnswerId self) => self.Value;
        public static implicit operator AnswerId(int value) => new(value);
    }
    public sealed record WebhookId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(WebhookId self) => self.Value;
        public static implicit operator WebhookId(Snowflake value) => new(value);
    }
    public sealed record WebhookToken(string Value) : RouteParameter
    {
        public static implicit operator string(WebhookToken self) => self.Value;
        public static implicit operator WebhookToken(string value) => new(value);
    }
    public sealed record GuildScheduledEventId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(GuildScheduledEventId self) => self.Value;
        public static implicit operator GuildScheduledEventId(Snowflake value) => new(value);
    }
    public sealed record RuleId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(RuleId self) => self.Value;
        public static implicit operator RuleId(Snowflake value) => new(value);
    }
    public sealed record RoleId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(RoleId self) => self.Value;
        public static implicit operator RoleId(Snowflake value) => new(value);
    }
    public sealed record EmojiId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(EmojiId self) => self.Value;
        public static implicit operator EmojiId(Snowflake value) => new(value);
    }
    public sealed record InteractionId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(InteractionId self) => self.Value;
        public static implicit operator InteractionId(Snowflake value) => new(value);
    }
    public sealed record InteractionToken(string Value) : RouteParameter
    {
        public static implicit operator string(InteractionToken self) => self.Value;
        public static implicit operator InteractionToken(string value) => new(value);
    }
    public sealed record OverwriteId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(OverwriteId self) => self.Value;
        public static implicit operator OverwriteId(Snowflake value) => new(value);
    }
    public sealed record Code(string Value) : RouteParameter
    {
        public static implicit operator string(Code self) => self.Value;
        public static implicit operator Code(string value) => new(value);
    }
    public sealed record SoundId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(SoundId self) => self.Value;
        public static implicit operator SoundId(Snowflake value) => new(value);
    }
    public sealed record IntegrationId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(IntegrationId self) => self.Value;
        public static implicit operator IntegrationId(Snowflake value) => new(value);
    }
    public sealed record StickerId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(StickerId self) => self.Value;
        public static implicit operator StickerId(Snowflake value) => new(value);
    }
    public sealed record PackId(Snowflake Value) : RouteParameter
    {
        public static implicit operator Snowflake(PackId self) => self.Value;
        public static implicit operator PackId(Snowflake value) => new(value);
    }
}