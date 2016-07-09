using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Discord.Net;
using System.Net;
using System;

namespace Discord.Tests.Framework.Responses.Users
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
            return Json.SerializeObject(User_Mocks.Me_User);
        }

        public static string Me_Bot_Valid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/users/@me' is GET.");
            if (TestRestClient.Headers["authorization"] != "Bot UserToken_VoltanaBot") throw new HttpException((HttpStatusCode)401);
            return Json.SerializeObject(User_Mocks.Bot_User);
        }

        public static string Id_User_Valid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/users/:id' is GET");
            return Json.SerializeObject(User_Mocks.Public_User);
        }

        public static string Id_User_Invalid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/users/:id' is GET");
            throw new HttpException((HttpStatusCode)404);
        }

        public static string Me_Guilds(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/users/@me/guilds' is GET");
            return Json.SerializeObject(Guilds.Guild_Mocks.UserGuildsList());
        }
    }

    public enum TestMode
    {
        User,
        Bot
    }
}
