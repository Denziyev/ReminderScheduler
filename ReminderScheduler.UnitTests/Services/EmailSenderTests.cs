using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using ReminderScheduler.Infrastructure.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderScheduler.UnitTests.Services
{
    public class EmailSenderTests
    {
        private readonly EmailSender _emailSender;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public EmailSenderTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            // Configure mock configuration if necessary

            _emailSender = new EmailSender(_mockConfiguration.Object);
        }

        [Fact]
        public async Task SendEmailAsync_ValidParameters_ShouldSendEmail()
        {
            // Arrange
            var to = "test@example.com";
            var subject = "Test Subject";
            var content = "Test Content";

            // Act
            var exception = await Record.ExceptionAsync(() => _emailSender.SendEmailAsync(to, subject, content));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task SendEmailAsync_InvalidEmail_ShouldThrowException()
        {
            // Arrange
            var to = "invalid-email";
            var subject = "Test Subject";
            var content = "Test Content";

            // Act
            var exception = await Record.ExceptionAsync(() => _emailSender.SendEmailAsync(to, subject, content));

            // Assert
            Assert.NotNull(exception);
        }
    }
}
