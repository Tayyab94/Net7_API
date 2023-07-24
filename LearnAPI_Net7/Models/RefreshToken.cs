using System.ComponentModel.DataAnnotations;

namespace LearnAPI_Net7.Models
{
    public class RefreshToken
    {
        [Key]
        public string userId { get;set; }
        [StringLength(500)]
        public string tokenId { get; set; }

        [StringLength(int.MaxValue)]
        public string refreshToken { get; set; }
    }
}
