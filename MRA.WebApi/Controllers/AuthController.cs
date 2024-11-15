using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MRA.DTO.Auth;
using MRA.DTO.JWT;
using MRA.DTO.ViewModels.Account;
using MRA.Services.Helpers;
using MRA.Services.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MRA.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        public const string ENV_WEB_ADMIN_USER = "ENV_WEB_ADMIN_USER";
        public const string ENV_WEB_ADMIN_PASSWORD = "ENV_WEB_ADMIN_PASSWORD";

        private readonly string _adminUser;
        private readonly string _adminPassword;

        private readonly JwtSettings _jwtSettings;

        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
            try
            {
                _adminUser = EnvironmentHelper.ReadValue(ENV_WEB_ADMIN_USER);
                _adminPassword = EnvironmentHelper.ReadValue(ENV_WEB_ADMIN_PASSWORD);

                _jwtSettings = JwtService.Load();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto.Username != _adminUser || loginDto.Password != _adminPassword)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginDto.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(1440),
                signingCredentials: creds);

            return Ok(new UserDto()
            {
                Username = loginDto.Username,
                Role = "admin",
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            }
            );
        }

        [HttpPost("validate-token")]
        //[Authorize]
        public IActionResult ValidateToken([FromBody] TokenDto tokenDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            try
            {
                tokenHandler.ValidateToken(tokenDto.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                }, out SecurityToken validatedToken);

                return Ok(true);
            }
            catch
            {
                return Unauthorized();
            }
        }
    }
}
