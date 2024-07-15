using Microsoft.Extensions.Configuration;
using Moq;
using ReminderScheduler.Infrastructure.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace ReminderScheduler.UnitTests.Services
{
    public class TelegramSenderTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ITelegramBotClient> _botClientMock;
        private readonly TelegramSender _telegramSender;

        public TelegramSenderTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _botClientMock = new Mock<ITelegramBotClient>();

            _configurationMock.Setup(c => c["Telegram:BotToken"]).Returns("test_bot_token");
            _telegramSender = new TelegramSender(_configurationMock.Object, _botClientMock.Object);
        }

        //[Fact]
        //public async Task SendTelegramMessageAsync_ValidParameters_ShouldCallSendTextMessageAsync()
        //{
        //    // Arrange
        //    var chatId = "1101059094";
        //    var message = "Hello World";

            
        //    //var chatIdObject = new ChatId(chatId);

        //    //_botClientMock
        //    //    .Setup(b => b.SendTextMessageAsync(
        //    //        It.Is<ChatId>(id => id.Identifier.ToString() == chatId),
        //    //        It.Is<string>(msg => msg == message),
        //    //        It.IsAny<int?>(),
        //    //        It.IsAny<ParseMode?>(),
        //    //        It.IsAny<IEnumerable<MessageEntity>>(),
        //    //        It.IsAny<bool?>(),
        //    //        It.IsAny<bool?>(),
        //    //        It.IsAny<bool?>(),
        //    //        It.IsAny<int?>(),
        //    //        It.IsAny<bool?>(),
        //    //        It.IsAny<IReplyMarkup>(),
        //    //        It.IsAny<CancellationToken>()))
        //    //    .ReturnsAsync(new Message());

        //    // Act
        //    await _telegramSender.SendTelegramMessageAsync(chatId, message);

        //    // Assert
        //    //_botClientMock
        //    //    .Verify(b => b.SendTextMessageAsync(
        //    //        It.Is<ChatId>(id => id.Identifier.ToString() == chatId),
        //    //        It.Is<string>(msg => msg == message),
        //    //        It.IsAny<int?>(),
        //    //        It.IsAny<ParseMode?>(),
        //    //        It.IsAny<IEnumerable<MessageEntity>>(),
        //    //        It.IsAny<bool?>(),
        //    //        It.IsAny<bool?>(),
        //    //        It.IsAny<bool?>(),
        //    //        It.IsAny<int?>(),
        //    //        It.IsAny<bool?>(),
        //    //        It.IsAny<IReplyMarkup>(),
        //    //        It.IsAny<CancellationToken>()),
        //    //    Times.Once());



        //    // Assert
        //    _botClientMock.Verify(
        //        client => client.SendTextMessageAsync(
        //            It.Is<ChatId>(id => id.Identifier == long.Parse(chatId)), // Assuming chatId is numeric
        //            message,
        //            null, // messageThreadId - use null or default value as per method signature
        //            null, // parseMode - use null or default value as per method signature
        //            null, // entities - use null or default value as per method signature
        //            null, // disableWebPagePreview - use null or default value as per method signature
        //            null, // disableNotification - use null or default value as per method signature
        //            null, // protectContent - use null or default value as per method signature
        //            null, // replyToMessageId - use null or default value as per method signature
        //            null, // allowSendingWithoutReply - use null or default value as per method signature
        //            null, // replyMarkup - use null or default value as per method signature
        //            default(CancellationToken) // cancellationToken - use default value
        //        ),
        //        Times.Once
        //    );
        //}

        [Fact]
        public async Task SendTelegramMessageAsync_NullMessage_ShouldThrowArgumentNullException()
        {
            // Arrange
            var chatId = "test_chat_id";
            string message = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _telegramSender.SendTelegramMessageAsync(chatId, message));
        }

        [Fact]
        public async Task SendTelegramMessageAsync_EmptyChatId_ShouldThrowArgumentException()
        {
            // Arrange
            var chatId = "";
            var message = "Hello, World!";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _telegramSender.SendTelegramMessageAsync(chatId, message));
        }

        [Fact]
        public void Constructor_NullBotClient_ShouldThrowArgumentNullException()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new TelegramSender(configurationMock.Object, null));
        }

        [Fact]
        public void Constructor_MissingBotToken_ShouldThrowArgumentException()
        {
            // Arrange
            var configurationMock = new Mock<IConfiguration>();
            var botClientMock = new Mock<ITelegramBotClient>();

            configurationMock.Setup(c => c["Telegram:BotToken"]).Returns((string)null);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new TelegramSender(configurationMock.Object, botClientMock.Object));
        }
    }
}
