using System.ComponentModel.DataAnnotations.Schema;

namespace LearnAPI_Net7.Models
{
    public class ProductImage
    {
        public string ProductCode { get;set; }

        [Column("ImageData",TypeName ="image")]
        public byte[] ImageData { get; set; }
    }
}
