using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.DTOs;
using AutoMapper;
using EXE_PET_HUB.Domain.Enums;

namespace EXE_PET_HUB.Application.Services
{
    public class AppointmentReminderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentReminderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<AppointmentReminderDto>> GetAllAsync()
        {
            var reminds = await _unitOfWork.AppointmentReminderRepository.GetAllAsync();

            return _mapper.Map<List<AppointmentReminderDto>>(reminds);
        }

        public async Task<AppointmentReminderDto?> GetByIdAsync(string id)
        {
            var remind = await _unitOfWork.AppointmentReminderRepository.GetByIdAsync(id);

            return remind == null ? null : _mapper.Map<AppointmentReminderDto>(remind);
        }

        public async Task<AppointmentReminderDto> GetByAppointmentIdAsync(string appointmentId)
        {
            var remind = await _unitOfWork.AppointmentReminderRepository.GetByAppointmentIdAsync(appointmentId);
            return _mapper.Map<AppointmentReminderDto>(remind);
        }

        public async Task<AppointmentReminderDto> CreateAsync(CreateAppointmentReminderDto dto)
        {
            var remind = _mapper.Map<AppointmentReminder>(dto);
            remind.Appointment =  await _unitOfWork.AppointmentRepository.GetByIdAsync(remind.AppointmentId);
            var reminds = await _unitOfWork.AppointmentReminderRepository.GetAllAsync();
            
            //Lọc điều kiện
            foreach (var a in reminds)
            {
                // 1 appointment chỉ có 1 remind
                if (a.AppointmentId == remind.AppointmentId)
                {
                    throw new Exception("Reminder for this appointment already exists.");
                }
            }
            // nếu appointment nhận vô có status khác Confirmed thì không được tạo remind
            if (remind.Appointment.Status != AppointmentStatus.Confirmed)
            {
                throw new Exception("Cannot create reminder for an appointment that is not confirmed.");
            }

            //nếu thời gian hiện tại đã vượt quá deadline để tạo remind (1 giờ trước thời gian bắt đầu của appointment) thì không được tạo remind
            DateTime appointmentDateTime = remind.Appointment.AppointmentDate.ToDateTime(remind.Appointment.StartTime);
            DateTime deadlineToCreateReminder = appointmentDateTime.AddHours(-1);
            if (DateTime.UtcNow.AddHours(7) >= deadlineToCreateReminder)
            {
                throw new Exception("Cannot create reminder less than 1 hour before the appointment.");
            }


            remind.Id = Guid.NewGuid().ToString();
            remind.CreatedAt = DateTime.UtcNow;
            remind.Status = ReminderStatus.Pending;
            remind.Appointment = null; // tránh vòng lặp tham chiếu khi map sang DTO
            await _unitOfWork.AppointmentReminderRepository.AddAsync(remind);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<AppointmentReminderDto>(remind);
        }

        public async Task<bool> Update(string id, UpdateAppointmentReminderDto dto)
        {
            var remind = await _unitOfWork.AppointmentReminderRepository.GetByIdAsync(id);
            if (remind == null) return false;
            var reminds = await _unitOfWork.AppointmentReminderRepository.GetAllAsync();

            //Lọc điều kiện
            foreach (var a in reminds)
            {
                if (a.AppointmentId == remind.AppointmentId && a.AppointmentId == dto.AppointmentId) continue; // bỏ qua remind đang update

                // 1 appointment chỉ có 1 remind
                if (a.AppointmentId == dto.AppointmentId)
                {
                    throw new Exception("Reminder for this appointment already exists.");
                }
            }
            // nếu appointment nhận vô có status khác Confirmed thì không được tạo remind
            var tmpAppoint = await _unitOfWork.AppointmentRepository.GetByIdAsync(dto.AppointmentId);
            if (tmpAppoint.Status != AppointmentStatus.Confirmed)
            {
                throw new Exception("Cannot update reminder for an appointment that is not confirmed.");
            }

            //nếu thời gian hiện tại đã vượt quá deadline để tạo remind (1 giờ trước thời gian bắt đầu của appointment) thì không được update remind
            DateTime appointmentDateTime = tmpAppoint.AppointmentDate.ToDateTime(tmpAppoint.StartTime);
            DateTime deadlineToCreateReminder = appointmentDateTime.AddHours(-1);
            if (DateTime.UtcNow.AddHours(7) >= deadlineToCreateReminder)
            {
                throw new Exception("Cannot update reminder less than 1 hour before the appointment.");
            }

            _mapper.Map(dto, remind);
            _unitOfWork.AppointmentReminderRepository.Update(remind);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> Delete(string id)
        {
            var remind = await _unitOfWork.AppointmentReminderRepository.GetByIdAsync(id);
            if (remind == null) return false;

            remind.Status = ReminderStatus.Failed;

            _unitOfWork.AppointmentReminderRepository.Update(remind);
            await _unitOfWork.CompleteAsync();
            return true;
        }


    }
}