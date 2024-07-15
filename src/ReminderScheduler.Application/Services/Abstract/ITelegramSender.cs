using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderScheduler.Application.Services.Abstract
{
    public interface ITelegramSender
    {
        Task SendTelegramMessageAsync(string chatId, string message);
    }
}
