using AutoMapper;
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Lấy danh sách item theo storeId.
        /// Manager (includeInactive = true) thấy tất cả kể cả item bị khóa.
        /// Customer (includeInactive = false) chỉ thấy item đang active.
        /// </summary>
        public async Task<List<ItemDto>> GetAllAsync(string storeId, bool includeInactive = false)
        {
            List<Item> items;
            if (includeInactive)
            {
                items = await _unitOfWork.ItemRepository.GetAllAsyncByStoreId(storeId);
            }
            else
            {
                items = await _unitOfWork.ItemRepository.FindAsyncByStoreId(storeId, x => x.IsActive);
            }
            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<ItemDto?> GetByIdAsync(string storeId, string id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsyncAndByStoreId(id, storeId);
            return item == null ? null : _mapper.Map<ItemDto>(item);
        }

        public async Task<ItemDto> CreateAsync(string storeId, CreateItemDto dto, String? imageurl)
        {
            var item = _mapper.Map<Item>(dto);
            item.Id = Guid.NewGuid().ToString();
            if (imageurl != null) { 
                item.ImageUrl = imageurl;
            }
            await _unitOfWork.ItemRepository.AddAsyncByStoreId(storeId, item);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ItemDto>(item);
        }

        public async Task<bool> UpdateAsync(string storeId, string id, UpdateItemDto dto, String? imageurl)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsyncAndByStoreId(id, storeId);
            if (item == null) return false;
            _mapper.Map(dto, item);
            if (imageurl != null)
            {
                item.ImageUrl = imageurl;
            }
            _unitOfWork.ItemRepository.UpdateByStoreId(storeId, item);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> LockAsync(string storeId, string id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsyncAndByStoreId(id, storeId);
            if (item == null) return false;
            item.IsActive = false;
            _unitOfWork.ItemRepository.UpdateByStoreId(storeId, item);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> UnlockAsync(string storeId, string id)
        {
            var item = await _unitOfWork.ItemRepository.GetByIdAsyncAndByStoreId(id, storeId);
            if (item == null) return false;
            item.IsActive = true;
            _unitOfWork.ItemRepository.UpdateByStoreId(storeId, item);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
