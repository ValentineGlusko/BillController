using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BillController.AuthenticationConfigs;
using BillController.Services.Abstraction;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BillController.Services.Implementation
{
    public class TokenJwtService : ITokenService
    {
        readonly  TokenSignConfigs _configuration;
        public TokenJwtService(IOptionsSnapshot<TokenSignConfigs> tokenConfigs)
        {
            _configuration = tokenConfigs.Value;
        }
        public string GenerateToken(string username)
        {
            var audience = _configuration.ValidAudiences;
            var secret =   _configuration.Secret;
            var issuer =   _configuration.ValidIssuer;
            if(string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(issuer))
            {
                throw new ApplicationException("Invalid configuration");
            }
            var signingkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var payload = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Audience = audience,
                Issuer = issuer,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(signingkey, SecurityAlgorithms.HmacSha256)
            };
            var tokem = new JwtSecurityTokenHandler().CreateToken(payload);
            return new JwtSecurityTokenHandler().WriteToken(tokem);
        }
    }

     
}
