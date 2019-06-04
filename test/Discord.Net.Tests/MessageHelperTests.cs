using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests for <see cref="Discord.Rest.MessageHelper"/> parsing.
    /// </summary>
    public class MessageHelperTests
    {
        /// <summary>
        ///     Tests that no tags work while they are in code blocks.
        /// </summary>
        [Theory]
        [InlineData("`@everyone`")]
        [InlineData("`<@&163184946742034432>`")]
        [InlineData("```@everyone```")]
        [InlineData("```cs \n @everyone```")]
        [InlineData("```cs <@&163184946742034432> ```")]
        [InlineData("``` test ``` ```cs <@&163184946742034432> ```")]
        public void NoTagsInCodeBlocks(string testData)
        {
            // don't care that I'm passing in null channels/guilds/users
            // as they shouldn't be required
            var result = Rest.MessageHelper.ParseTags(testData, null, null, null);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("`` <@&163184946742034432>")]
        [InlineData("``` test ``` ``` test ``` <@&163184946742034432>")]
        public void TagsWork(string testData) // todo better names
        {
            // don't care that I'm passing in null channels/guilds/users
            // as they shouldn't be required
            var result = Rest.MessageHelper.ParseTags(testData, null, null, null);
            Assert.NotEmpty(result);
        }

    }
}

