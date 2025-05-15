using NET_BE.Model;

namespace NET_BE.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<PagedModel<T>> GetPagedAsync(int pageIndex, int pageSize);
        Task<T?> GetByIdAsync(string id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
    }
}
