using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.Model
{
    public class Image
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public string Path { get; set; }
    }
}