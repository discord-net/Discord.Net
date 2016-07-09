using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API;

namespace Discord.Tests.Framework.Responses.Roles
{
    public static class Constant_Role_Mocks
    {
        public static Role Mock_Everyone_Role
        {
            get
            {
                return new Role
                {
                    Id = 66078535390867456,
                    Color = 0,
                    Position = 0,
                    Hoist = false,
                    Managed = false,
                    Name = "@everyone",
                    Permissions = 36953089
                };
            }
        }
    }
}
