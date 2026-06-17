namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> CompleteAsync();
        IPetRepository PetRepository { get; }
        IMedicalRecordRepository MedicalRecordRepository { get; }
        IItemRepository ItemRepository { get; }
        IInvoiceRepository InvoiceRepository { get; }
        IAppointmentRepository AppointmentRepository { get; }
        IAppointmentReminderRepository AppointmentReminderRepository { get; }
        IUserRepository UserRepository { get; }
        IStorePackageRepository StorePackageRepository { get; }
        IStoreRepository StoreRepository { get; }
    }
}
