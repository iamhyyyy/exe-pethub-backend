using AutoMapper;
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Cấu hình map 2 chiều giữa Pet và PetDto
            CreateMap<Pet, PetDto>().ReverseMap();

            // Cấu hình map 2 chiều cho MedicalRecord
            CreateMap<MedicalRecord, MedicalRecordDto>().ReverseMap();
        }
    }
}