using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using BillController.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace BillController.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, IConfiguration configuration) : ControllerBase
    {
 
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] AddOrUpdateAppUserModel model)
        {
            if (ModelState.IsValid)
            {
                var existing = await userManager.FindByNameAsync(model.UserName);
                if (existing != null)
                {
                    ModelState.AddModelError("", "User is Already Taken");
                    return BadRequest(ModelState);
                }
                var user = new AppUser()
                {
                    
                    UserName = model.UserName,
                    Email = model.Email,
                   // SecurityStamp = Guid.NewGuid().ToString(),
                };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = GenerateToken(model.UserName);
                    return Ok(new { token });
                }
                foreach (var error in result.Errors) 
                {
                    ModelState.AddModelError("", error.Description);
                }
                
            }
            return BadRequest(ModelState);
        }

 
        public string GenerateToken(string userName)
        {
            var audience = configuration["JwtConfig:ValidAudiences"];
            var secret  = configuration["JwtConfig:Secret"];
            var issuer = configuration["JwtConfig:ValidIssuer"];
            if(audience == null || secret == null || issuer == null)
            {
                throw new ApplicationException("There is no token data");
            }
            var claims = new Claim(ClaimTypes.Name, userName);
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims  = new Dictionary<string, object>(){{ClaimTypes.Name, userName}},
                Audience = audience,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var handler  = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            var write = handler.WriteToken(token);
            return write;
        }
    }

}



