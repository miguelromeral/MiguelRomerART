using MRA.DTO.JWT;
using MRA.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRA.Services.JWT
{
    public class JwtService
    {
        public const string ENV_WEB_JWT_ISSUER = "ENV_WEB_JWT_ISSUER";
        public const string ENV_WEB_JWT_AUDIENCE = "ENV_WEB_JWT_AUDIENCE";
        public const string ENV_WEB_JWT_KEY = "ENV_WEB_JWT_KEY";

        public static JwtSettings Load()
        {
            return new JwtSettings()
            {
                Issuer = EnvironmentHelper.ReadValue(ENV_WEB_JWT_ISSUER),
                Audience = EnvironmentHelper.ReadValue(ENV_WEB_JWT_AUDIENCE),
                Key = EnvironmentHelper.ReadValue(ENV_WEB_JWT_KEY)
            };
        }
    }
}
