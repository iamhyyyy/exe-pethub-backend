using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Infrastructure.Data;
using System.Collections;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private Hashtable? _repositories;
        private IPetRepository? _petRepository;
        private IMedicalRecordRepository? _medicalRecordRepository;
        private IItemRepository? _itemRepository;
        private IAppointmentReminderRepository? _appointmentReminderRepository;
        private IAppointmentRepository? appointmentRepository;
        private IInvoiceRepository? _invoiceRepository;
        private IUserRepository? _userRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            _repositories ??= new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance =
                    Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<T>)_repositories[type]!;
        }

        public IPetRepository PetRepository => _petRepository ??= new PetRepository(_context);
        public IMedicalRecordRepository MedicalRecordRepository => _medicalRecordRepository ??= new MedicalRecordRepository(_context);
        public IItemRepository ItemRepository => _itemRepository ??= new ItemRepository(_context);
        public IAppointmentReminderRepository AppointmentReminderRepository => _appointmentReminderRepository ??= new AppointmentReminderRepository(_context);
        public IAppointmentRepository AppointmentRepository => appointmentRepository ??= new AppointmentRepository(_context);
        public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);
        public IInvoiceRepository InvoiceRepository => _invoiceRepository ??= new InvoiceRepository(_context);
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
