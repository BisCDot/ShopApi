using Microsoft.EntityFrameworkCore;
using Shopping_Cart_Api.Data;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.Service
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProduct()
        {
            return await _context.Products.Include(pro => pro.Image).ToArrayAsync();
        }

        public async Task<Product> GetProductById(Guid id)
        {
            return await _context.Products.Include(pro => pro.Image).Where(m => m.Id == id).SingleOrDefaultAsync();
        }

        public async Task<bool> DeleteProduct(Guid id)
        {
            _context.Products.Remove(await _context.Products.FindAsync(id));
            return await _context.SaveChangesAsync() == 1;
        }

        public async Task<string> AddProduct(ProductViewModel product)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var message = "";

            var entity = new Product
            {
                Id = new Guid(),
                ProductCode = product.ProductCode,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
            };

            _context.Products.Add(entity);

            foreach (var img in product.Image)
            {
                var extention = Path.GetExtension(img.FileName);
                if (allowedExtensions.Contains(extention.ToLower()) || img.Length > 2000000)
                    message = "Select jpg or jpeg or png less than 2Μ";
                var fileName = Path.Combine("Products", DateTime.Now.Ticks + extention);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", fileName);

                try
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }
                }
                catch
                {
                    return "can not upload image";
                }

                var imageEntity = new Image
                {
                    Id = new Guid(),
                    ProductId = entity.Id,
                    Path = fileName
                };

                _context.Images.Add(imageEntity);
            };

            bool success = await _context.SaveChangesAsync() == 1 + product.Image.Count;

            if (success) return entity.Id.ToString();
            else return message;
        }

        public async Task<bool> DeleteProductById(Guid id)
        {
            _context.Products.Remove(await _context.Products.FindAsync(id));
            return 1 == await _context.SaveChangesAsync();
        }

        public async Task<string> EditProductById(Guid id, int stock)
        {
            var entity = await _context.Products.FindAsync(id);

            if (entity.Stock - stock >= 0)
                entity.Stock = entity.Stock - stock;
            else return "Unsucessfull";

            bool success = await _context.SaveChangesAsync() == 1;
            if (success) return entity.Id.ToString();
            else return "Unsucessfull";
        }
    }
}