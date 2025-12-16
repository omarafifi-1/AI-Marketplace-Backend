using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.DTOs
{
    public class GetMessageDTO
    {
        //public int SessionId { get; set; }
        public string Content { get; set; }
        public DateTime EndAt { get; set; }
    }
}
