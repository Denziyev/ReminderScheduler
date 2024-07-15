using ReminderScheduler.Application.DTOs.Reminder;
using ReminderScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderScheduler.Application.Services.Abstract
{
    public interface IReminderService
    {
        Task<List<ReminderDto>> GetAllRemindersAsync();
        Task<ReminderDto> GetReminderByIdAsync(int id);
        Task<ReminderDto> AddReminderAsync(CreateReminderDto createReminderDto);
        Task UpdateReminderAsync(int id, UpdateReminderDto updateReminderDto);
        Task DeleteReminderAsync(int id);
        Task DeleteBulkReminderAsync(IEnumerable<int> Ids);
    }
}
