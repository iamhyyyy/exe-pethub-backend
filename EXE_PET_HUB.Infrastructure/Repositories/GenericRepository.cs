using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, IStoreEntity
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<List<T>> GetAllAsyncByStoreId(string storeId)
        {
            return await _dbSet.Where(a => a.StoreId == storeId).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<T?> GetByIdAsyncAndByStoreId(string id, string storeId)
        {
            return await _dbSet.FirstOrDefaultAsync(a => EF.Property<string>(a, "Id") == id && a.StoreId == storeId);
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<List<T>> FindAsyncByStoreId(string storeId, Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(a => a.StoreId == storeId).Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddAsyncByStoreId(string storeId, T entity)
        {
            entity.StoreId = storeId;
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateByStoreId(string storeId, T entity)
        {
            if (entity.StoreId != storeId)
                throw new UnauthorizedAccessException("Không có quyền cập nhật dữ liệu của store khác!");

            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteByStoreId(string storeId, T entity)
        {
            if (entity.StoreId != storeId)
                throw new UnauthorizedAccessException("Không có quyền xóa dữ liệu của store khác!");

            _dbSet.Remove(entity);
        }
    }
}
