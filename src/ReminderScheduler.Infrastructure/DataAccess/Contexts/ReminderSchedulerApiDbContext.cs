using Microsoft.EntityFrameworkCore;
using ReminderScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderScheduler.Infrastructure.DataAccess.Contexts
{
    public class ReminderSchedulerApiDbContext:DbContext
    {
        public ReminderSchedulerApiDbContext(DbContextOptions<ReminderSchedulerApiDbContext> options) : base(options) { }

        public DbSet<Reminder> Reminders { get; set; }
    }
}
