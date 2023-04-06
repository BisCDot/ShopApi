using Microsoft.AspNetCore.Http;
using Shopping_Cart_Api.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.ViewModels
{
    public class ProductViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "The max product length must be between {2} and {1}", MinimumLength = 1)]
        public string ProductName { get; set; }

        [Required]
        [DataType(DataType.Currency, ErrorMessage = "The price must be currency")]
        public double Price { get; set; }

        public List<IFormFile> Image { get; set; }
        public int CategoryId { get; set; }
        public List<string> ProductDetails { get; set; }
    }
}