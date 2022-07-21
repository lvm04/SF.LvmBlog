
namespace SF.BlogData.Repository;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAll();
    Task<T> Get(int id);
    Task Create(T item);
    Task Update(T item);
    Task Delete(T item);
}
