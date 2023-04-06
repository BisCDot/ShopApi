using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.Model
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public List<Image> Image { get; set; }
        public ICollection<ProductDetail> ProductDetails { get; set; }
    }
}