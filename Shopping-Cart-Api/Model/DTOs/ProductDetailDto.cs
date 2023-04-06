using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Shopping_Cart_Api.ViewModels;

namespace Shopping_Cart_Api.Model.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public string ProductName { get; set; }

        public double Price { get; set; }

        public List<string> Images { get; set; }
        public List<ProductDetailDto> ProductDetailDtos { get; set; }
    }

    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public double Price { get; set; }
        public string Size { get; set; }

        public int Quantity { get; set; }
        public List<Image> Images { get; set; }
    }
}