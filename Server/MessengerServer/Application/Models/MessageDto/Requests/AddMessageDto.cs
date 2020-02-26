﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Models.MessageDto
{
    public class AddMessageDto
    {
        [Required]
        public string Content { get; set; }

        public int userId { get; set; }

        [Required]
        public int chatId { get; set; }
    }
}
