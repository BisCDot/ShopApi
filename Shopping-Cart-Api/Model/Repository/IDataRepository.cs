namespace Shopping_Cart_Api.Model.Repository
{
    public interface IDataRepository<TEntity>
    {
        IEnumerable<TEntity> GetAll();

        void Add(TEntity entity);
    }
}