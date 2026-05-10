namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> CompleteAsync();
        IPetRepository PetRepository { get; }
    }
}
