using AutoMapper;
using ReminderScheduler.Application.DTOs.Reminder;
using ReminderScheduler.Application.Services.Abstract;
using ReminderScheduler.Domain.Entities;
using ReminderScheduler.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReminderScheduler.Application.Services.Concrete
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRepository _reminderRepository;
        private readonly IMapper _mapper;

        public ReminderService(IReminderRepository reminderRepository, IMapper mapper)
        {
            _reminderRepository = reminderRepository;
            _mapper = mapper;
        }

        public async Task<List<ReminderDto>> GetAllRemindersAsync()
        {
            var reminders = await _reminderRepository.GetAllAsync();
            return _mapper.Map<List<ReminderDto>>(reminders);
        }

        public async Task<ReminderDto> GetReminderByIdAsync(int id)
        {
            var reminder = await _reminderRepository.GetByIdAsync(id);
            return _mapper.Map<ReminderDto>(reminder);
        }

        public async Task<ReminderDto> AddReminderAsync(CreateReminderDto createReminderDto)
        {
            var reminder = _mapper.Map<Reminder>(createReminderDto);
            await _reminderRepository.AddAsync(reminder);
            return _mapper.Map<ReminderDto>(reminder);
        }

        public async Task UpdateReminderAsync(int id, UpdateReminderDto updateReminderDto)
        {
            var existingReminder = await _reminderRepository.GetByIdAsync(id);

            if (existingReminder == null)
            {
                throw new ArgumentException($"Reminder with id {id} not found.");
            }

            // Update properties from updateReminderDto
            existingReminder.To = updateReminderDto.To;
            existingReminder.Content = updateReminderDto.Content;
            existingReminder.SendAt = updateReminderDto.SendAt;
            existingReminder.Method = updateReminderDto.Method;

            await _reminderRepository.UpdateAsync(existingReminder);
        }

        public async Task DeleteReminderAsync(int id)
        {
            await _reminderRepository.DeleteAsync(id);
        }

        public async Task DeleteBulkReminderAsync(IEnumerable<int> Ids)
        {
            await _reminderRepository.DeleteRangeAsync(Ids);
        }
    }
}
