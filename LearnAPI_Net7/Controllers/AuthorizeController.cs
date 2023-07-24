using LearnAPI_Net7.ContaxtFiles;
using LearnAPI_Net7.Models;
using LearnAPI_Net7.Models.ViewModels;
using LearnAPI_Net7.Models.ViewModels.ModelsHelpers;
using LearnAPI_Net7.Services;
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
        private readonly IRefreshHandler _refreshHandler;

        public AuthorizeController(LearnDataContaxt context,IOptions<JWTSettings>options,
            IRefreshHandler refreshHandler)
        {
            this._refreshHandler= refreshHandler;
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
                    Expires = DateTime.UtcNow.AddMinutes(3),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokendesc);
                var finalToken = tokenHandler.WriteToken(token);

                return Ok(new TokenResponseVM()
                { Token= finalToken, RefreshToken = await this._refreshHandler.GenerateToken(dbUser.Name)});
            }else
            {
                return Unauthorized();
            }
        }


        [HttpPost("GenerateRefreshtoken")]
        public async Task<IActionResult> GenerateRefreshtoken([FromBody] TokenResponseVM model)
        {
            var refreshToken = await this._context.RefreshTokens.FirstOrDefaultAsync(s => s.refreshToken.Equals(model.RefreshToken));
            if (refreshToken != null)
            {
                // Generate Token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(this.jWTSettings.securityKey);

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(model.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out securityToken);

                var _token =securityToken as JwtSecurityToken;
                if(_token != null&& _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    string userName = principal.Identity?.Name;
                    var existData = await this._context.RefreshTokens.FirstOrDefaultAsync(s => s.userId == userName
                    && s.refreshToken == model.RefreshToken);
                    if(existData != null)
                    {
                        var newToken = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(50),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jWTSettings.securityKey))
                                        , SecurityAlgorithms.HmacSha256));

                        var _finalToken = tokenHandler.WriteToken(newToken);
                        return Ok(new TokenResponseVM()
                        { Token = _finalToken, RefreshToken = await this._refreshHandler.GenerateToken(userName) });
                    }else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }

                //var tokendesc = new SecurityTokenDescriptor
                //{
                //    Subject = new ClaimsIdentity(new Claim[]
                //    {
                //        new Claim(ClaimTypes.Name, User.Name),
                //        new Claim(ClaimTypes.Role, dbUser.role)
                //    }),
                //    Expires = DateTime.UtcNow.AddSeconds(30),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                //};

                //var token = tokenHandler.CreateToken(tokendesc);
                //var finalToken = tokenHandler.WriteToken(token);

                //return Ok(new TokenResponseVM()
                //{ Token = finalToken, RefreshToken = await this._refreshHandler.GenerateToken(dbUser.Name) });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
