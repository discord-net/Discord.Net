using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Discord
{
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
        public void TestNullOrWhitespaceToken(string token)
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
        public void TestWebhookTokenDoesNotThrowExceptions(string token)
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
        public void TestBearerTokenDoesNotThrowExceptions(string token)
        {
            TokenUtils.ValidateToken(TokenType.Bearer, token);
        }

        // No tests for invalid bearer token behavior, because there is nothing there yet.

        /// <summary>
        ///     Tests the behavior of <see cref="TokenUtils.ValidateToken(TokenType, string)"/>
        ///     to see that valid Bot tokens do not throw Exceptions.
        ///     Valid Bot tokens can be strings of length 59 or above.
        /// </summary>
        [Theory]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWs")]
        [InlineData("This appears to be completely invalid, however the current validation rules are not very strict.")]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWss")]
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWsMTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWs")]
        public void TestBotTokenDoesNotThrowExceptions(string token)
        {
            // This example token is pulled from the Discord Docs
            // https://discordapp.com/developers/docs/reference#authentication-example-bot-token-authorization-header
            // should not throw any exception
            TokenUtils.ValidateToken(TokenType.Bot, token);
        }

        /// <summary>
        ///     Tests the usage of <see cref="TokenUtils.ValidateToken(TokenType, string)"/> with
        ///     a Bot token that is invalid.
        /// </summary>
        [Theory]
        [InlineData("This is invalid")]
        // missing a single character from the end
        [InlineData("MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKW")]
        // bearer token
        [InlineData("6qrZcUqja7812RVdnEKjpzOL4CvHBFG")]
        // client secret
        [InlineData("937it3ow87i4ery69876wqire")]
        public void TestBotTokenInvalidThrowsArgumentException(string token)
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
        // TokenType.User
        [InlineData(0)]
        // out of range TokenType
        [InlineData(4)]
        [InlineData(7)]
        public void TestUnrecognizedTokenType(int type)
        {
            Assert.Throws<ArgumentException>(() =>
                TokenUtils.ValidateToken((TokenType)type, "MTk4NjIyNDgzNDcxOTI1MjQ4.Cl2FMQ.ZnCjm1XVW7vRze4b7Cq4se7kKWs"));
        }
    }
}
