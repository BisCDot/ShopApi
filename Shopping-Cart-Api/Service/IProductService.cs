using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.Service
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProduct();

        Task<Product> GetProductById(Guid id);

        Task<string> AddProduct(ProductViewModel tag);

        Task<bool> DeleteProductById(Guid id);
    }
}