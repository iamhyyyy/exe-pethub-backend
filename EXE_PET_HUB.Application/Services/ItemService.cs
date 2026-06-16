using AutoMapper;
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.Services
{
    public class ItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ItemService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<ItemDto>> GetAllAsync(string storeId)
        {
            var items = await _unitOfWork.ItemRepository.GetAllAsyncByStoreId(storeId);
            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<ItemDto?> GetByIdAsync(string storeId, string id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsyncAndByStoreId(id, storeId);
            return item == null ? null : _mapper.Map<ItemDto>(item);
        }

        public async Task<ItemDto> CreateAsync(string storeId, CreateItemDto dto)
        {
            var item = _mapper.Map<Item>(dto);
            item.Id = Guid.NewGuid().ToString();
            await _unitOfWork.ItemRepository.AddAsyncByStoreId(storeId, item);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ItemDto>(item);
        }

        public async Task<bool> UpdateAsync(string storeId, string id, UpdateItemDto dto)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsyncAndByStoreId(id, storeId);
            if (item == null) return false;
            _mapper.Map(dto, item);
            _unitOfWork.ItemRepository.UpdateByStoreId(storeId, item);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
