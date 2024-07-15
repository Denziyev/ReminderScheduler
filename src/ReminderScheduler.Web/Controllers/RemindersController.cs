using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReminderScheduler.Application.DTOs.Reminder;
using ReminderScheduler.Application.Services.Abstract;
using ReminderScheduler.Domain.Entities;

namespace ReminderScheduler.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateReminderDto> _createReminderValidator;

        public ReminderController(IReminderService reminderService, IMapper mapper, IValidator<CreateReminderDto> createReminderValidator)
        {
            _reminderService = reminderService;
            _mapper = mapper;
            _createReminderValidator = createReminderValidator;
        }

        // GET: api/reminder
        [HttpGet("GetAllReminders")]
        public async Task<ActionResult<List<ReminderDto>>> GetAllReminders()
        {

                var reminders = await _reminderService.GetAllRemindersAsync();
                return Ok(reminders);

        }

        // GET: api/reminder/{id}
        [HttpGet("GetReminderById/{id}")]
        public async Task<ActionResult<ReminderDto>> GetReminderById(int id)
        {
                var reminder = await _reminderService.GetReminderByIdAsync(id);
                if (reminder == null)
                {
                    return NotFound();
                }
                return Ok(reminder);

        }

        // POST: api/reminder
        [HttpPost("AddReminder")]
        public async Task<IActionResult> AddReminder([FromBody] CreateReminderDto model)
        {
            ValidationResult result = await _createReminderValidator.ValidateAsync(model);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            var createdReminder=  await _reminderService.AddReminderAsync(model);

                // Since CreateReminderDto doesn't have an Id property, we return 201 Created
                // and provide the route to get the newly created resource by its own identifier
                return CreatedAtAction(nameof(GetReminderById), new { id = createdReminder.Id }, createdReminder);

        }

        // PUT: api/reminder/{id}
        [HttpPut("UpdateReminder/{id}")]
        public async Task<IActionResult> UpdateReminder(int id, [FromBody] UpdateReminderDto model)
        {
            if (!ModelState.IsValid || id != model.Id)
            {
                return BadRequest(ModelState);
            }


                await _reminderService.UpdateReminderAsync(id, model);
                return NoContent();

        }

        // DELETE: api/reminder/{id}
        [HttpDelete("DeleteBulkReminder")]
        public async Task<IActionResult> DeleteBulkReminder([FromBody]IEnumerable<int> Ids)
        {

                await _reminderService.DeleteBulkReminderAsync(Ids);
                return NoContent();

        }
    }

}
