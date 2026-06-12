using EXE_PET_HUB.Domain.Entities;
using System.Linq.Expressions;

namespace EXE_PET_HUB.Application.Interfaces
{
    // 1. Dành cho TẤT CẢ mọi bảng (kể cả bảng không có StoreId)
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(string id);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }

    // 2. Chỉ dành riêng cho các bảng CÓ StoreId
    public interface IStoreGenericRepository<T> : IGenericRepository<T> where T : class, IStoreEntity
    {
        Task<List<T>> GetAllAsyncByStoreId(string storeId);
        Task<T?> GetByIdAsyncAndByStoreId(string id, string storeId);
        Task<List<T>> FindAsyncByStoreId(string storeId, Expression<Func<T, bool>> predicate);
        Task AddAsyncByStoreId(string storeId, T entity);
        void UpdateByStoreId(string storeId, T entity);
        void DeleteByStoreId(string storeId, T entity);
    }
}
