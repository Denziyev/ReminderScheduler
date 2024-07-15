using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ReminderScheduler.Application.DTOs;
using ReminderScheduler.Application.DTOs.Reminder;
using ReminderScheduler.Application.Services.Abstract;
using ReminderScheduler.Domain.Entities;
using ReminderScheduler.Web.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReminderScheduler.UnitTests.Controllers;
public class ReminderControllerTests
{
    private readonly Mock<IReminderService> _mockReminderService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IValidator<CreateReminderDto>> _mockValidator;
    private readonly ReminderController _controller;

    public ReminderControllerTests()
    {
        _mockReminderService = new Mock<IReminderService>();
        _mockMapper = new Mock<IMapper>();
        _mockValidator = new Mock<IValidator<CreateReminderDto>>();
        _controller = new ReminderController(_mockReminderService.Object, _mockMapper.Object, _mockValidator.Object);
    }

    [Fact]
    public async Task GetAllReminders_ReturnsOkResult()
    {
        // Arrange
        var mockReminders = new List<ReminderDto>(); // mock data
        _mockReminderService.Setup(service => service.GetAllRemindersAsync()).ReturnsAsync(mockReminders);

        // Act
        var result = await _controller.GetAllReminders();

        // Assert
        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetAllReminders_ReturnsReminders()
    {
        // Arrange
        var mockReminders = new List<ReminderDto>
        {
            new ReminderDto { Id = 1, Content = "Reminder 1" },
            new ReminderDto { Id = 2, Content = "Reminder 2" }
        }; // mock data
        _mockReminderService.Setup(service => service.GetAllRemindersAsync()).ReturnsAsync(mockReminders);

        // Act
        var result = await _controller.GetAllReminders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var reminders = Assert.IsAssignableFrom<List<ReminderDto>>(okResult.Value);
        Assert.Equal(2, reminders.Count);
    }

    [Fact]
    public async Task GetReminderById_ReturnsOkResult()
    {
        // Arrange
        var mockReminderId = 1;
        var mockReminder = new ReminderDto { Id = mockReminderId, Content = "Reminder 1" };
        _mockReminderService.Setup(service => service.GetReminderByIdAsync(mockReminderId)).ReturnsAsync(mockReminder);

        // Act
        var result = await _controller.GetReminderById(mockReminderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var reminder = Assert.IsType<ReminderDto>(okResult.Value);
        Assert.Equal(mockReminderId, reminder.Id);
    }

    [Fact]
    public async Task GetReminderById_ReturnsNotFoundResult()
    {
        // Arrange
        var mockReminderId = 1;
        _mockReminderService.Setup(service => service.GetReminderByIdAsync(mockReminderId)).ReturnsAsync((ReminderDto)null);

        // Act
        var result = await _controller.GetReminderById(mockReminderId);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddReminder_ValidModel_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var validModel = new CreateReminderDto { Content = "New Reminder" };
        _mockValidator.Setup(validator => validator.ValidateAsync(validModel, default)).ReturnsAsync(new ValidationResult());

        var createdReminderId = 1;
        var createdReminder = new ReminderDto { Id = createdReminderId, Content = validModel.Content };
        _mockReminderService.Setup(service => service.AddReminderAsync(validModel)).ReturnsAsync(createdReminder);

        // Act
        var result = await _controller.AddReminder(validModel);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ReminderController.GetReminderById), createdAtActionResult.ActionName);
        Assert.Equal(createdReminderId, createdAtActionResult.RouteValues["id"]);
        Assert.Equal(createdReminder, createdAtActionResult.Value);
    }

    [Fact]
    public async Task AddReminder_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var invalidModel = new CreateReminderDto(); // invalid model
        var validationFailures = new List<ValidationFailure> { new ValidationFailure("Title", "Title is required.") };
        var validationResult = new ValidationResult(validationFailures);
        _mockValidator.Setup(validator => validator.ValidateAsync(invalidModel, default)).ReturnsAsync(validationResult);

        // Act
        var result = await _controller.AddReminder(invalidModel);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var errors = Assert.IsType<List<ValidationFailure>>(badRequestResult.Value);
        Assert.Single(errors);
        Assert.Equal("Title is required.", errors[0].ErrorMessage);
    }

    [Fact]
    public async Task UpdateReminder_ValidModel_ReturnsNoContent()
    {
        // Arrange
        var reminderId = 1;
        var validModel = new UpdateReminderDto { Id = reminderId, Content = "Updated Reminder" };
        _controller.ModelState.Clear(); // Clear ModelState for valid state
        _mockReminderService.Setup(service => service.UpdateReminderAsync(reminderId, validModel));

        // Act
        var result = await _controller.UpdateReminder(reminderId, validModel);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateReminder_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        var reminderId = 1;
        var invalidModel = new UpdateReminderDto { Id = reminderId, Content = null }; // invalid model
        _controller.ModelState.AddModelError("Title", "Title is required.");

        // Act
        var result = await _controller.UpdateReminder(reminderId, invalidModel);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task DeleteBulkReminder_ReturnsNoContentResult()
    {
        // Arrange
        var ids = new List<int> { 1, 2, 3 };
        _mockReminderService.Setup(service => service.DeleteBulkReminderAsync(It.IsAny<IEnumerable<int>>()))
                            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteBulkReminder(ids);

        // Assert
        var noContentResult = Assert.IsType<NoContentResult>(result);
        _mockReminderService.Verify(service => service.DeleteBulkReminderAsync(ids), Times.Once);
    }
}
