using Discord.Rest;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests for <see cref="MessageHelper"/> parsing.
    /// </summary>
    public class MessageHelperTests
    {
        /// <summary>
        ///     Tests that no tags are parsed while in code blocks
        ///     or inline code.
        /// </summary>
        [Theory]
        [InlineData("`@everyone`")]
        [InlineData("`<@163184946742034432>`")]
        [InlineData("```@everyone```")]
        [InlineData("```cs \n @everyone```")]
        [InlineData("```cs <@163184946742034432> ```")]
        [InlineData("``` test ``` ```cs <@163184946742034432> ```")]
        [InlineData("`<:test:537920404019216384>`")]
        [InlineData("``` @everyone  `")] // discord client handles these weirdly
        [InlineData("``` @everyone  ``")]
        [InlineData("` @here `")]
        [InlineData("` @everyone @here <@163184946742034432> <@&163184946742034432> <#163184946742034432> <:test:537920404019216384> `")]
        public void ParseTagsInCode(string testData)
        {
            // don't care that I'm passing in null channels/guilds/users
            // as they shouldn't be required
            var result = MessageHelper.ParseTags(testData, null, null, null);
            Assert.Empty(result);
        }

        /// <summary> Tests parsing tags that surround inline code or a code block. </summary>
        [Theory]
        [InlineData("`` <@&163184946742034432>")]
        [InlineData("``` code block 1 ``` ``` code block 2 ``` <@&163184946742034432>")]
        [InlineData("` code block 1 ``` ` code block 2 ``` <@&163184946742034432>")]
        [InlineData("<@&163184946742034432> ``` code block 1 ```")]
        [InlineData("``` code ``` ``` code ``` @here ``` code ``` ``` more ```")]
        [InlineData("``` code ``` @here ``` more ```")]
        public void ParseTagsAroundCode(string testData)
        {
            // don't care that I'm passing in null channels/guilds/users
            // as they shouldn't be required
            var result = MessageHelper.ParseTags(testData, null, null, null);
            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData(@"\` @everyone \`")]
        [InlineData(@"\`\`\` @everyone \`\`\`")]
        [InlineData(@"hey\`\`\`@everyone\`\`\`!!")]
        public void IgnoreEscapedCodeBlocks(string testData)
        {
            var result = MessageHelper.ParseTags(testData, null, null, null);
            Assert.NotEmpty(result);
        }

        // cannot test parsing a user, as it uses the ReadOnlyCollection<IUser> arg.
        // this could be done if mocked entities are merged in PR #1290

        /// <summary> Tests parsing a mention of a role. </summary>
        [Theory]
        [InlineData("<@&163184946742034432>")]
        [InlineData("**<@&163184946742034432>**")]
        [InlineData("__<@&163184946742034432>__")]
        [InlineData("<><@&163184946742034432>")]
        public void ParseRole(string roleTag)
        {
            var result = MessageHelper.ParseTags(roleTag, null, null, null);
            Assert.Contains(result, x => x.Type == TagType.RoleMention);
        }

        /// <summary> Tests parsing a channel. </summary>
        [Theory]
        [InlineData("<#429115823748284417>")]
        [InlineData("**<#429115823748284417>**")]
        [InlineData("<><#429115823748284417>")]
        public void ParseChannel(string channelTag)
        {
            var result = MessageHelper.ParseTags(channelTag, null, null, null);
            Assert.Contains(result, x => x.Type == TagType.ChannelMention);
        }

        /// <summary> Tests parsing an emoji. </summary>
        [Theory]
        [InlineData("<:test:537920404019216384>")]
        [InlineData("**<:test:537920404019216384>**")]
        [InlineData("<><:test:537920404019216384>")]
        public void ParseEmoji(string emoji)
        {
            var result = MessageHelper.ParseTags(emoji, null, null, null);
            Assert.Contains(result, x => x.Type == TagType.Emoji);
        }

        /// <summary> Tests parsing a mention of @everyone. </summary>
        [Theory]
        [InlineData("@everyone")]
        [InlineData("**@everyone**")]
        public void ParseEveryone(string everyone)
        {
            var result = MessageHelper.ParseTags(everyone, null, null, null);
            Assert.Contains(result, x => x.Type == TagType.EveryoneMention);
        }

        /// <summary> Tests parsing a mention of @here. </summary>
        [Theory]
        [InlineData("@here")]
        [InlineData("**@here**")]
        public void ParseHere(string here)
        {
            var result = MessageHelper.ParseTags(here, null, null, null);
            Assert.Contains(result, x => x.Type == TagType.HereMention);
        }
    }
}
