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
            CreateMap<CreatePetDto, Pet>();
            CreateMap<UpdatePetDto, Pet>();

            // Cấu hình map 2 chiều cho MedicalRecord
            CreateMap<MedicalRecord, MedicalRecordDto>().ReverseMap();
            CreateMap<CreateMedicalRecordDto, MedicalRecord>();
            CreateMap<UpdateMedicalRecordDto, MedicalRecord>();

            //cấu hình map 2 chiều cho Item
            CreateMap<Item, ItemDto>().ReverseMap();
            CreateMap<CreateItemDto, Item>();
            CreateMap<UpdateItemDto, Item>();

            // Cấu hình map 2 chiều cho appointment
            CreateMap<Appointment, AppointmentDto>().ReverseMap();
            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<UpdateAppointmentDto, Appointment>();

            
            CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.PetName,
                opt => opt.MapFrom(src => src.Pet!.Name))
            .ForMember(dest => dest.AppointmentNote,
                opt => opt.MapFrom(src => src.Appointment!.AppointmentNote))
            .ForMember(dest => dest.CustomerName,
                opt => opt.MapFrom(src => src.Customer!.UserName));

            CreateMap<CreateInvoiceDetailDto, Invoice>();

            CreateMap<Invoice, ResponseInvoiceOfCreateDto>()
                .ForMember(dest => dest.PetName,
                opt => opt.MapFrom(src => src.Pet!.Name))
                .ForMember(dest => dest.AppointmentNote,
                    opt => opt.MapFrom(src => src.Appointment!.AppointmentNote))
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer!.UserName));

            CreateMap<InvoiceDetail, InvoiceDetailsDto>();

            CreateMap<CreateInvoiceDetailDto, InvoiceDetail>();
        }
    }
}