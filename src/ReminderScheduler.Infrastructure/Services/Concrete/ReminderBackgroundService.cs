using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReminderScheduler.Application.Services.Abstract;

public class ReminderBackgroundService : BackgroundService
{
    private readonly ILogger<ReminderBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ReminderBackgroundService(
        ILogger<ReminderBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Reminder Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Checking for reminders to send.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                var telegramService = scope.ServiceProvider.GetRequiredService<ITelegramSender>();
                var reminders = await reminderService.GetAllRemindersAsync();
                var now = DateTime.UtcNow;

                foreach (var reminder in reminders.Where(r => r.SendAt <= now))
                {
                    try
                    {
                        if (reminder.Method == "email")
                        {
                            await emailService.SendEmailAsync(reminder.To, "Reminder", reminder.Content);
                        }
                        else if (reminder.Method == "telegram")
                        {
                            await telegramService.SendTelegramMessageAsync(reminder.To, reminder.Content);
                        }

                        await reminderService.DeleteReminderAsync(reminder.Id);
                        _logger.LogInformation($"Reminder sent and deleted: {reminder.Id}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error sending reminder: {reminder.Id}");
                    }
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        _logger.LogInformation("Reminder Background Service is stopping.");
    }
}
