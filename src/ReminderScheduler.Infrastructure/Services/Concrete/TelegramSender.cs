using Microsoft.Extensions.Configuration;
using ReminderScheduler.Application.Services.Abstract;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ReminderScheduler.Infrastructure.Services.Concrete
{
    public class TelegramSender : ITelegramSender
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramSender(IConfiguration configuration, ITelegramBotClient botClient)
        {
            _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
            var botToken = configuration["Telegram:BotToken"];
            if (string.IsNullOrEmpty(botToken))
            {
                throw new ArgumentException("Telegram Bot Token is not configured properly.");
            }
        }

        public async Task SendTelegramMessageAsync(string chatId, string message)
        {
            if (string.IsNullOrEmpty(chatId))
            {
                throw new ArgumentException("Chat ID cannot be null or empty.", nameof(chatId));
            }
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message), "Message cannot be null or empty.");
            }

            await _botClient.SendTextMessageAsync(new ChatId(chatId), message);
        }
    }
}
