﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderScheduler.Application.DTOs.Reminder
{
    public class ReminderDto
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string Content { get; set; }
        public DateTime SendAt { get; set; }
        public string Method { get; set; }
    }
}
