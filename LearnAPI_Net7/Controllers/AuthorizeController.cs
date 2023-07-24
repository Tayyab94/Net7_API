using LearnAPI_Net7.ContaxtFiles;
using LearnAPI_Net7.Models.ViewModels;
using LearnAPI_Net7.Models.ViewModels.ModelsHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearnAPI_Net7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly LearnDataContaxt _context;
        private readonly JWTSettings jWTSettings;

        public AuthorizeController(LearnDataContaxt context,IOptions<JWTSettings>options)
        {
            this.jWTSettings = options.Value;
            _context= context;
        }

        [HttpPost("LoginUser")]
        public async Task<IActionResult> LoginUser([FromBody]LoginUserVM model)
        {
            var dbUser =await this._context.Users.FirstOrDefaultAsync(s=>s.Code==model.Code && s.Password.Equals(model.Password));
            if(dbUser != null)
            {
                // Generate Token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(this.jWTSettings.securityKey);
                var tokendesc = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, dbUser.Name),
                        new Claim(ClaimTypes.Role, dbUser.role)
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokendesc);
                var finalToken = tokenHandler.WriteToken(token);

                return Ok(finalToken);
            }else
            {
                return Unauthorized();
            }
        }
    }
}
