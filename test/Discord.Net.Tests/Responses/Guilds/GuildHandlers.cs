using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Tests.Framework.Responses.Guilds
{
    public static class GuildHandlers
    {
        public static string Id_Valid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/guilds/:id' is GET.");
            return Json.SerializeObject(Guild_Mocks.Guild_From_Id);
        }

        public static string Id_Invalid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/guilds/:id' is GET.");
            throw new Net.HttpException((HttpStatusCode)404);
        }

        public static string Bans_Valid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/guilds/:id/bans' is GET.");
            return Json.SerializeObject(Guild_Mocks.GuildBansList);
        }

        public static string Members_Valid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/guilds/:id/members' is GET.");
            return Json.SerializeObject(Member_Mocks.Guild_Members);
        }

        public static string Member_Valid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/guilds/:id/members/:member_id' is GET.");
            return Json.SerializeObject(Member_Mocks.Guild_Member_1);
        }

        public static string Member_Invalid(string method, string json)
        {
            Assert.AreEqual("GET", method, "Expected method to '/guilds/:id/members/:member_id' is GET.");
            throw new Net.HttpException((HttpStatusCode)404);
        }
    }
}
