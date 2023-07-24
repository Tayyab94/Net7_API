using LearnAPI_Net7.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI_Net7.ContaxtFiles
{
    public class LearnDataContaxt :DbContext
    {
        public LearnDataContaxt(DbContextOptions<LearnDataContaxt> options):base(options)
        {
            
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<User> Users { get; set; }  
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
