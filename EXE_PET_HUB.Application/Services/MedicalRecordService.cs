using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.DTOs;
using AutoMapper;

namespace EXE_PET_HUB.Application.Services
{
    
    public class MedicalRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MedicalRecordService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<MedicalRecordDto>> GetAllAsync()
        {
            var records = await _unitOfWork.MedicalRecordRepository.GetAllAsync();

            return _mapper.Map<List<MedicalRecordDto>>(records);
        }

        public async Task<MedicalRecordDto?> GetByIdAsync(string id)
        {
            var record = await _unitOfWork.MedicalRecordRepository.GetByIdAsync(id);

            return record == null ? null : _mapper.Map<MedicalRecordDto>(record);
        }

        public async Task<MedicalRecordDto> CreateAsync(CreateMedicalRecordDto dto)
        {

            var record = _mapper.Map<MedicalRecord>(dto);
            record.Id = Guid.NewGuid().ToString();
            await _unitOfWork.MedicalRecordRepository.AddAsync(record);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<MedicalRecordDto>(record);
        }

        public async Task<bool> Update(string id, UpdateMedicalRecordDto dto)
        {
            var record = await _unitOfWork.MedicalRecordRepository.GetByIdAsync(id);
            if (record == null) return false;

            _mapper.Map(dto, record);

            _unitOfWork.MedicalRecordRepository.Update(record);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
