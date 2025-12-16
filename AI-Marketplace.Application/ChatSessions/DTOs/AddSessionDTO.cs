using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.DTOs
{
    public class AddSessionDTO
    {
        public DateTime StartAt { get; set; }= DateTime.UtcNow;
        public int UserId { get; set; } = 0;
    }
}
