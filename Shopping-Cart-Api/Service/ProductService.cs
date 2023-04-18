using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping_Cart_Api.Data;
using Shopping_Cart_Api.Model;
using Shopping_Cart_Api.Model.DTOs;
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
        private readonly IMapper _mapper;

        public ProductService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProduct()
        {
            var productDetails = await _context.ProductDetails.ProjectTo<ProductDetailDto>(_mapper.ConfigurationProvider).ToListAsync();
            var group = productDetails.GroupBy(i => i.ProductId);
            var result = new List<ProductDto>();
            foreach (var item in group)
            {
                result.Add(new ProductDto
                {
                    Id = item.First().ProductId,
                    ProductName = item.First().ProductName,
                    Price = item.First().Price,
                    Images = item.First().Images,
                    ProductDetailDtos = item.ToList().Select(i => new ProductDetailDto { Id = i.Id, Size = i.Size, Quantity = i.Quantity }).ToList()
                });
            }

            return result;
        }

        public async Task<ProductDto> GetProductById(Guid id)
        {
            var productById = await _context.ProductDetails.Where(i => i.Products.Id == id).ProjectTo<ProductDetailDto>(_mapper.ConfigurationProvider).ToArrayAsync();
            var group = productById;
            var result = new ProductDto();
            foreach (var item in productById)
            {
                result = new ProductDto()
                {
                    Id = item.Id,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Images = item.Images,
                    ProductDetailDtos = productById.ToList().Select(i => new ProductDetailDto { Id = i.Id, Size = i.Size, Quantity = i.Quantity }).ToList(),
                };
            }

            return result;
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
                Id = Guid.NewGuid(),
                ProductName = product.ProductName,
                Price = product.Price,
                CategoryId = product.CategoryId,
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
            string valueString = string.Join(",", product.ProductDetails);
            var sizes = JsonConvert.DeserializeObject<List<ProductDetail>>(valueString);
            if (sizes != null)
            {
                foreach (var item in sizes)
                {
                    var productDetailEntity = new ProductDetail()
                    {
                        Id = Guid.NewGuid(),
                        ProductId = entity.Id,
                        Size = item.Size,
                        Quantity = item.Quantity
                    };
                    _context.ProductDetails.Add(productDetailEntity);
                }
            }

            bool success = await _context.SaveChangesAsync() == 1 + product.Image.Count + sizes.Count;

            if (success) return entity.Id.ToString();
            else return message;
        }

        public async Task<bool> DeleteProductById(Guid id)
        {
            _context.Products.Remove(await _context.Products.FindAsync(id));
            return 1 == await _context.SaveChangesAsync();
        }

        //public async Task<string> EditProductById(Guid id, int stock)
        //{
        //    var entity = await _context.Products.FindAsync(id);

        //    //if (entity.Stock - stock >= 0)
        //    //    entity.Stock = entity.Stock - stock;
        //    //else return "Unsucessfull";

        //    //bool success = await _context.SaveChangesAsync() == 1;
        //    //if (success) return entity.Id.ToString();
        //    //else return "Unsucessfull";
        //}
    }
}