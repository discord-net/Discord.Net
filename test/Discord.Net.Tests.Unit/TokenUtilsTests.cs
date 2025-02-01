using System;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests for the <see cref="Discord.TokenUtils"/> methods.
    /// </summary>
    public class TokenUtilsTests
    {
        /// <summary>
        ///     Tests the usage of <see cref="TokenUtils.ValidateToken(TokenType, string)"/>
        ///     to see that when a null, empty or whitespace-only string is passed as the token,
        ///     it will throw an ArgumentNullException.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")] // string.Empty isn't a constant type
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("\t")]
        public void NullOrWhitespaceToken(string token)
        {
            // an ArgumentNullException should be thrown, regardless of the TokenType
            Assert.Throws<ArgumentNullException>(() => TokenUtils.ValidateToken(TokenType.Bearer, token));
            Assert.Throws<ArgumentNullException>(() => TokenUtils.ValidateToken(TokenType.Bot, token));
            Assert.Throws<ArgumentNullException>(() => TokenUtils.ValidateToken(TokenType.Webhook, token));
        }

        /// <summary>
        ///     Tests the behavior of <see cref="TokenUtils.ValidateToken(TokenType, string)"/>
        ///     to see that valid Webhook tokens do not throw Exceptions.
        /// </summary>
        /// <param name="token"></param>
        [Theory]
        [InlineData("123123123")]
        // bot token
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWs")]
        // bearer token taken from discord docs
        [InlineData("6qrZcUqja7812RVdnEKjpzOL4CvHBFG")]
        // client secret
        [InlineData("937it3ow87i4ery69876wqire")]
        public void WebhookTokenDoesNotThrowExceptions(string token)
        {
            TokenUtils.ValidateToken(TokenType.Webhook, token);
        }

        // No tests for invalid webhook token behavior, because there is nothing there yet.

        /// <summary>
        ///     Tests the behavior of <see cref="TokenUtils.ValidateToken(TokenType, string)"/>
        ///     to see that valid Webhook tokens do not throw Exceptions.
        /// </summary>
        /// <param name="token"></param>
        [Theory]
        [InlineData("123123123")]
        // bot token
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWs")]
        // bearer token taken from discord docs
        [InlineData("6qrZcUqja7812RVdnEKjpzOL4CvHBFG")]
        // client secret
        [InlineData("937it3ow87i4ery69876wqire")]
        public void BearerTokenDoesNotThrowExceptions(string token)
        {
            TokenUtils.ValidateToken(TokenType.Bearer, token);
        }

        // No tests for invalid bearer token behavior, because there is nothing there yet.

        /// <summary>
        ///     Tests the behavior of <see cref="TokenUtils.ValidateToken(TokenType, string)"/>
        ///     to see that valid Bot tokens do not throw Exceptions.
        ///     Valid Bot tokens can be strings of length 58 or above.
        /// </summary>
        [Theory]
        // missing a single character from the end, 58 char. still should be valid
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKW")]
        // 59 char token
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWs")]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWss")]
        public void BotTokenDoesNotThrowExceptions(string token)
        {
            // This example token is pulled from the Discord Docs
            // https://discord.com/developers/docs/reference#authentication-example-bot-token-authorization-header
            // should not throw any exception
            TokenUtils.ValidateToken(TokenType.Bot, token);
        }

        /// <summary>
        ///     Tests the usage of <see cref="TokenUtils.ValidateToken(TokenType, string)"/> with
        ///     a Bot token that is invalid.
        /// </summary>
        [Theory]
        [InlineData("This is invalid")]
        // bearer token
        [InlineData("6qrZcUqja7812RVdnEKjpzOL4CvHBFG")]
        // client secret
        [InlineData("937it3ow87i4ery69876wqire")]
        // 57 char bot token
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kK")]
        // ends with invalid characters
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7k ")]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7k\n")]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7k\t")]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7k\r\n")]
        // starts with invalid characters
        [InlineData(" MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7k")]
        [InlineData("\nMTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7k")]
        [InlineData("\tMTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7k")]
        [InlineData("\r\nMTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7k")]
        [InlineData("This is an invalid token, but it passes the check for string length.")]
        // valid token, but passed in twice
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWsMTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWs")]
        public void BotTokenInvalidThrowsArgumentException(string token)
        {
            Assert.Throws<ArgumentException>(() => TokenUtils.ValidateToken(TokenType.Bot, token));
        }

        /// <summary>
        ///     Tests the behavior of <see cref="TokenUtils.ValidateToken(TokenType, string)"/>
        ///     to see that an <see cref="ArgumentException"/> is thrown when an invalid
        ///     <see cref="TokenType"/> is supplied as a parameter.
        /// </summary>
        /// <remarks>
        ///     The <see cref="TokenType.User"/> type is treated as an invalid <see cref="TokenType"/>.
        /// </remarks>
        [Theory]
        // out of range TokenType
        [InlineData(-1)]
        [InlineData(4)]
        [InlineData(7)]
        public void UnrecognizedTokenType(int type)
        {
            Assert.Throws<ArgumentException>(() =>
                TokenUtils.ValidateToken((TokenType)type, "MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWs"));
        }

        /// <summary>
        ///     Checks the <see cref="TokenUtils.CheckBotTokenValidity(string)"/> method for expected output.
        /// </summary>
        /// <param name="token"> The Bot Token to test.</param>
        /// <param name="expected"> The expected result. </param>
        [Theory]
        // this method only checks the first part of the JWT
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4..", true)]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kK", true)]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4. this part is invalid. this part is also invalid", true)]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.", false)]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4", false)]
        [InlineData("NDI4NDc3OTQ0MDA5MTk1NTIw.xxxx.xxxxx", true)]
        // should not throw an unexpected exception
        [InlineData("", false)]
        [InlineData(null, false)]
        public void CheckBotTokenValidity(string token, bool expected)
        {
            Assert.Equal(expected, TokenUtils.CheckBotTokenValidity(token));
        }

        [Theory]
        // cannot pass a ulong? as a param in InlineData, so have to have a separate param
        // indicating if a value is null
        [InlineData("NDI4NDc3OTQ0MDA5MTk1NTIw", false, 428477944009195520)]
        // should return null w/o throwing other exceptions
        [InlineData("", true, 0)]
        [InlineData(" ", true, 0)]
        [InlineData(null, true, 0)]
        [InlineData("these chars aren't allowed @U#)*@#!)*", true, 0)]
        public void DecodeBase64UserId(string encodedUserId, bool isNull, ulong expectedUserId)
        {
            var result = TokenUtils.DecodeBase64UserId(encodedUserId);
            if (isNull)
                Assert.Null(result);
            else
                Assert.Equal(expectedUserId, result);
        }
    }
}
