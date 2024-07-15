using AutoMapper;
using ReminderScheduler.Application.DTOs.Reminder;
using ReminderScheduler.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReminderScheduler.Infrastructure.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reminder, ReminderDto>();
            CreateMap<CreateReminderDto, Reminder>();
            CreateMap<UpdateReminderDto, Reminder>();
        }
    }
}
