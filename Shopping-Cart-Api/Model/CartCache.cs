using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Cart_Api.Model
{
    public class CartCache
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; }

        public string? ProductName { get; set; }
        public string? Image { get; set; }
        public int? Price { get; set; }
        public int? Quantity { get; set; }
    }
}