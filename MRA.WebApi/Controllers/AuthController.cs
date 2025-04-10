using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MRA.DTO.Auth;
using MRA.Infrastructure.Configuration;
using MRA.DTO.ViewModels.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MRA.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly AppConfiguration _appConfig;

        public AuthController(AppConfiguration appConfig)
        {
            _appConfig = appConfig;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginDto)
        {
            if (loginDto.Username != _appConfig.Administrator.User || loginDto.Password != _appConfig.Administrator.Password)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginDto.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _appConfig.Jwt.Issuer,
                audience: _appConfig.Jwt.Audience,
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
        public IActionResult ValidateToken([FromBody] TokenDto tokenDto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appConfig.Jwt.Key);

            try
            {
                tokenHandler.ValidateToken(tokenDto.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _appConfig.Jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _appConfig.Jwt.Audience,
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
