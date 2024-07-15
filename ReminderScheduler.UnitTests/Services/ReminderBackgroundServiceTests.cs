
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using ReminderScheduler.Application.DTOs.Reminder;
using ReminderScheduler.Application.Services.Abstract;
using Xunit;
using IEmailSender = ReminderScheduler.Application.Services.Abstract.IEmailSender;

namespace ReminderScheduler.UnitTests.Services;
public class ReminderBackgroundServiceTests
{
    private readonly Mock<ILogger<ReminderBackgroundService>> _loggerMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mock<IEmailSender> _emailServiceMock;
    private readonly Mock<ITelegramSender> _telegramServiceMock;
    private readonly Mock<IReminderService> _reminderServiceMock;
    private readonly ReminderBackgroundService _backgroundService;
    private readonly Mock<IServiceScope> _scopeMock;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;

    public ReminderBackgroundServiceTests()
    {
        _loggerMock = new Mock<ILogger<ReminderBackgroundService>>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _emailServiceMock = new Mock<IEmailSender>();
        _telegramServiceMock = new Mock<ITelegramSender>();
        _reminderServiceMock = new Mock<IReminderService>();
        _scopeMock = new Mock<IServiceScope>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_scopeMock.Object);
        _scopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(_scopeFactoryMock.Object);

        _backgroundService = new ReminderBackgroundService(
            _loggerMock.Object,
            _serviceProviderMock.Object);

        _serviceProviderMock.Setup(x => x.GetService(typeof(IReminderService))).Returns(_reminderServiceMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(IEmailSender))).Returns(_emailServiceMock.Object);
        _serviceProviderMock.Setup(x => x.GetService(typeof(ITelegramSender))).Returns(_telegramServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldSendReminders()
    {
        // Arrange
        var reminders = new List<ReminderDto>
        {
            new ReminderDto { Id = 1, To = "test@example.com", Content = "Email Reminder", SendAt = DateTime.UtcNow.AddSeconds(-1), Method = "email" },
            new ReminderDto { Id = 2, To = "123456789", Content = "Telegram Reminder", SendAt = DateTime.UtcNow.AddSeconds(-1), Method = "telegram" }
        };

        _reminderServiceMock.Setup(x => x.GetAllRemindersAsync()).ReturnsAsync(reminders);

        var stoppingTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var stoppingToken = stoppingTokenSource.Token;

        // Act
        var backgroundTask = _backgroundService.StartAsync(stoppingToken);
        await Task.Delay(2000); // Allow some time for the background service to process

        // Assert
        _emailServiceMock.Verify(x => x.SendEmailAsync("test@example.com", "Reminder", "Email Reminder"), Times.Once);
        _telegramServiceMock.Verify(x => x.SendTelegramMessageAsync("123456789", "Telegram Reminder"), Times.Once);
        _reminderServiceMock.Verify(x => x.DeleteReminderAsync(1), Times.Once);
        _reminderServiceMock.Verify(x => x.DeleteReminderAsync(2), Times.Once);

        stoppingTokenSource.Cancel();
        await backgroundTask; // Ensure the background task completes
    }
}
