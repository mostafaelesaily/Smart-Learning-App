using Microsoft.EntityFrameworkCore;
using Smart_Learning_App.Data;
using System.Linq.Expressions;

namespace Smart_Learning_App.Repo_Uow.Main
{
    public class MainRepo<T> : IRepo<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public MainRepo(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<T> AddItemAsync(T item)
        {
            var addedItem = await _dbSet.AddAsync(item);
            return addedItem.Entity;
        }

        public Task<int> CountAsync()
        {
            return _dbSet.CountAsync();
        }

        public async Task<T?> DeleteItemAsync(int id)
        {
            var item = await _dbSet.FindAsync(id);

            if (item == null)
                return null;

            _dbSet.Remove(item);
            return item;
        }

        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> filter)
        {
           return await _dbSet.Where(filter).ToListAsync();
        }

        public async Task<T?> FindElmentAsync(Expression<Func<T, bool>> filter)
        {
          return await _dbSet.FirstOrDefaultAsync(filter);
        }

        public async Task<List<T>> GetAllData()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<List<T>> GetAllPaged(int pageNum, int pageSize)
        {
            return await _dbSet
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetByIdsStringAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<T> Query()
        {
            return _dbSet;
        }

        public Task UpdateItemAsync(T item)
        {
            _dbSet.Update(item);
            return Task.CompletedTask;
        }
    }
}