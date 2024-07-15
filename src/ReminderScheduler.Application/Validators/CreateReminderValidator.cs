using FluentValidation;
using ReminderScheduler.Application.DTOs.Reminder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderScheduler.Application.Validators
{
    public class CreateReminderValidator : AbstractValidator<CreateReminderDto>
    {
        public CreateReminderValidator()
        {
            RuleFor(x => x.To)
                .NotEmpty().WithMessage("Recipient is required.")
                .When(x => x.Method == "email")
                .EmailAddress().WithMessage("Invalid email address format.")
                .When(x => x.Method == "email");

            RuleFor(x => x.To)
                .NotEmpty().WithMessage("Chat ID is required for Telegram reminders.")
                .When(x => x.Method == "telegram");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.");

            RuleFor(x => x.SendAt)
                .GreaterThan(DateTime.UtcNow).WithMessage("SendAt must be in the future.");

            RuleFor(x => x.Method)
                .NotEmpty().WithMessage("Method is required.")
                .Must(m => m == "email" || m == "telegram")
                .WithMessage("Method must be either 'email' or 'telegram'.");
        }
    }
}
