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
        public async Task<List<ItemDto>> GetAllAsync()
        {
            var items = await _unitOfWork.Repository<Item>().GetAllAsync();
            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<ItemDto?> GetByIdAsync(string id)
        {
            var item = await _unitOfWork.Repository<Item>().GetByIdAsync(id);
            return item == null ? null : _mapper.Map<ItemDto>(item);
        }

        public async Task<ItemDto> CreateAsync(CreateItemDto dto)
        {
            var item = _mapper.Map<Item>(dto);
            item.Id = Guid.NewGuid().ToString();
            await _unitOfWork.Repository<Item>().AddAsync(item);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ItemDto>(item);
        }

        public async Task<bool> UpdateAsync(string id, UpdateItemDto dto)
        {
            var item = await _unitOfWork.Repository<Item>().GetByIdAsync(id);
            if (item == null) return false;
            _mapper.Map(dto, item);
            _unitOfWork.Repository<Item>().Update(item);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var item = await _unitOfWork.Repository<Item>().GetByIdAsync(id);
            if (item == null) return false;
            _unitOfWork.Repository<Item>().Delete(item);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
