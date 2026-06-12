using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    // 1. Base Generic Repository xử lý các hàm cơ bản
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task<T?> GetByIdAsync(string id) => await _dbSet.FindAsync(id);
        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
    }

    // 2. Store Generic Repository xử lý các hàm liên quan StoreId
    public class StoreGenericRepository<T> : GenericRepository<T>, IStoreGenericRepository<T> where T : class, IStoreEntity
    {
        public StoreGenericRepository(AppDbContext context) : base(context) { }

        public async Task<List<T>> GetAllAsyncByStoreId(string storeId)
        {
            return await _dbSet.Where(a => a.StoreId == storeId).ToListAsync();
        }

        public async Task<T?> GetByIdAsyncAndByStoreId(string id, string storeId)
        {
            return await _dbSet.FirstOrDefaultAsync(a => EF.Property<string>(a, "Id") == id && a.StoreId == storeId);
        }

        public async Task<List<T>> FindAsyncByStoreId(string storeId, Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(a => a.StoreId == storeId).Where(predicate).ToListAsync();
        }

        public async Task AddAsyncByStoreId(string storeId, T entity)
        {
            entity.StoreId = storeId;
            await _dbSet.AddAsync(entity);
        }

        public void UpdateByStoreId(string storeId, T entity)
        {
            if (entity.StoreId != storeId) throw new UnauthorizedAccessException("Không có quyền chỉnh sửa!");
            _dbSet.Update(entity);
        }

        public void DeleteByStoreId(string storeId, T entity)
        {
            if (entity.StoreId != storeId) throw new UnauthorizedAccessException("Không có quyền xóa!");
            _dbSet.Remove(entity);
        }
    }
}