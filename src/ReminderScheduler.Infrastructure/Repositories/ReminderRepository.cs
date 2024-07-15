using Microsoft.EntityFrameworkCore;
using ReminderScheduler.Domain.Entities;
using ReminderScheduler.Domain.Repositories;
using ReminderScheduler.Infrastructure.DataAccess.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderScheduler.Infrastructure.Repositories
{
    public class ReminderRepository : IReminderRepository
    {
        private readonly ReminderSchedulerApiDbContext _context;

        public ReminderRepository(ReminderSchedulerApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reminder>> GetAllAsync()
        {
            return await _context.Reminders.Where(x=>!x.IsSent).ToListAsync();
        }

        public async Task<Reminder> GetByIdAsync(int id)
        {
            return await _context.Reminders.FindAsync(id);
        }

        public async Task AddAsync(Reminder reminder)
        {
            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reminder reminder)
        {
            _context.Entry(reminder).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder != null)
            {
                _context.Reminders.Remove(reminder);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteRangeAsync(IEnumerable<int> ids)
        {
            var reminders = await _context.Reminders
                                          .Where(r => ids.Contains(r.Id))
                                          .ToListAsync();
            if (reminders.Any())
            {
                _context.Reminders.RemoveRange(reminders);
                await _context.SaveChangesAsync();
            }
        }

    }
}
