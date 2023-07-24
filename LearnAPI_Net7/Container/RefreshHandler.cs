using LearnAPI_Net7.ContaxtFiles;
using LearnAPI_Net7.Models;
using LearnAPI_Net7.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LearnAPI_Net7.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly LearnDataContaxt _context;
        public RefreshHandler(LearnDataContaxt context)
        {
            _context = context;
        }

        public async Task<string> GenerateToken(string userName)
        {
            var randomNumber = new Byte[32];
            using(var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);

                string refreshToken= Convert.ToBase64String(randomNumber);
                var ExistToken= await this._context.RefreshTokens.FirstOrDefaultAsync(s=>s.userId== userName);
                if (ExistToken!=null)
                {
                    ExistToken.refreshToken = refreshToken;
                }else
                {
                    await this._context.RefreshTokens.AddAsync(new RefreshToken
                    {
                        userId = userName,
                        refreshToken = refreshToken,
                        tokenId= new Random().Next().ToString()
                    });
                }
                await this._context.SaveChangesAsync();
                return refreshToken;
            }   
        }
    }
}
