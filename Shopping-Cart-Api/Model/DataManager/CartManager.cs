using Shopping_Cart_Api.Data;
using Shopping_Cart_Api.Model.Repository;
using StackExchange.Redis;

namespace Shopping_Cart_Api.Model.DataManager
{
    public class CartManager : IDataRepository<CartCache>
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public CartManager(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<CartCache> GetAll()
        {
            return _applicationDbContext.CartCaches.ToList();
        }

        public void Add(CartCache entity)
        {
            _applicationDbContext.CartCaches.Add(entity);
            _applicationDbContext.SaveChanges();
        }
    }
}