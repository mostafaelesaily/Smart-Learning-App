using System.Linq.Expressions;

namespace Smart_Learning_App.Repo_Uow
{
    public interface IRepo<T> where T : class
    {
        Task<List<T>> GetAllPaged(int pageNum, int pageSize);
        Task<List<T>> FindAllAsync(Expression<Func<T,bool>> filter);
        Task<List<T>> GetAllData();
        IQueryable<T> Query();
        Task<int> CountAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> FindElmentAsync(Expression<Func<T, bool>> filter);
        Task<T?> GetByIdsStringAsync(string id);
        Task<T> AddItemAsync(T item);
        Task UpdateItemAsync(T item);
        Task<T?> DeleteItemAsync(int id);
    }
}