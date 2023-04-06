using System.ComponentModel.DataAnnotations;

namespace Shopping_Cart_Api.Model
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}