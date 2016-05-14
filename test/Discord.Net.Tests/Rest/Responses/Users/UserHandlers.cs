using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Discord.Net;
using System.Net;
using System;

namespace Discord.Tests.Rest.Responses.Users
{
    public static class UserHandlers
    {
        public static TestMode Mode;

        public static string Me_Handler(string method, string json)
        {
            switch (Mode)
            {
                case TestMode.User:
                    return Me_User_Valid(method, json);
                case TestMode.Bot:
                    return Me_Bot_Valid(method, json);
                default:
                    throw new ArgumentException("TestMode was set incorrectly.");
            }
        }

        public static string Me_User_Valid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/users/@me' is GET.");
            if (TestRestClient.Headers["authorization"] != "UserToken_Voltana") throw new HttpException((HttpStatusCode)401);
            return JsonConvert.SerializeObject(new Mock_Me_User_Valid());
        }

        public static string Me_Bot_Valid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/users/@me' is GET.");
            if (TestRestClient.Headers["authorization"] != "Bot UserToken_VoltanaBot") throw new HttpException((HttpStatusCode)401);
            return JsonConvert.SerializeObject(new Mock_Me_User_Valid());
        }
    }

    public enum TestMode
    {
        User,
        Bot
    }
}
