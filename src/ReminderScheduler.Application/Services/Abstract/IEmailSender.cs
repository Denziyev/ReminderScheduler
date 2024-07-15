using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderScheduler.Application.Services.Abstract
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string content);
    }
}
