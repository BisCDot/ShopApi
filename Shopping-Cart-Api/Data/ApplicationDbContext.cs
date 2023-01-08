using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping_Cart_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping_Cart_Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<CartCache> CartCaches { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<Product>().HasData(
            //    new Product()
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductCode = "1",
            //        ProductName = "Cosy Crew Neck Jumper",
            //        Price = 575000,
            //    },
            //    new Product()
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductCode = "2",
            //        ProductName = "Áo hoodie ấm áp",
            //        Price = 244000,
            //    },
            //    new Product()
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductCode = "3",
            //        ProductName = "Bộ âu phục nữ",
            //        Price = 1004000,
            //    },
            //    new Product()
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductCode = "4",
            //        ProductName = "Hoodie Chất rắn",
            //        Price = 262000,
            //    },
            //    new Product()
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductCode = "5",
            //        ProductName = "Áo khoác Nam",
            //        Price = 444800,
            //    },
            //    new Product()
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductCode = "6",
            //        ProductName = "ROMWE Quần thể thao",
            //        Price = 332000,
            //    },
            //    new Product()
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductCode = "7",
            //        ProductName = "Áo choàng Nam ",
            //        Price = 847000,
            //    },
            //    new Product()
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductCode = "7",
            //        ProductName = "Áo khoác Trench ",
            //        Price = 884000,
            //    }

            //    );
            base.OnModelCreating(builder);
        }
    }
}