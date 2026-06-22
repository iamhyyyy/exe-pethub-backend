using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.DTOs;
using AutoMapper;

namespace EXE_PET_HUB.Application.Services
{
    public class StoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StoreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<StoreDto>> GetAllAsync()
        {
            var stores = await _unitOfWork.StoreRepository.GetAllAsync();
            return _mapper.Map<List<StoreDto>>(stores);
        }

        public async Task<List<StoreDtoForCustomer>> GetAllForCustomerAsync()
        {
            var stores = await _unitOfWork.StoreRepository.GetAllAsync();
            var activeStores = stores.Where(s => s.IsActive == true).ToList();
            return _mapper.Map<List<StoreDtoForCustomer>>(activeStores);
        }

        public async Task<StoreDto?> GetByIdAsync(string id)
        {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(id);
            return store == null ? null : _mapper.Map<StoreDto>(store);
        }

        public async Task<StoreDto> CreateAsync(CreateStoreDto dto)
        {
            var store = _mapper.Map<Store>(dto);
            store.Id = Guid.NewGuid().ToString();
            await _unitOfWork.StoreRepository.AddAsync(store);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<StoreDto>(store);
        }

        public async Task<bool> Update(string id, UpdateStoreDto dto)
        {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(id);
            if (store == null) return false;

            _mapper.Map(dto, store);

            _unitOfWork.StoreRepository.Update(store);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> Delete(string id)
        {
            var store = await _unitOfWork.StoreRepository.GetByIdAsync(id);
            if (store == null) return false;
            store.IsActive = false;

            _unitOfWork.StoreRepository.Update(store);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}