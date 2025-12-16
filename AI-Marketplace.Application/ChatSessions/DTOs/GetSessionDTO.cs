using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.DTOs
{
    public class GetSessionDTO
    {
        public int SessionId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int UserId { get; set; } = 0;
        public List<GetMessageDTO> ChatMessages { get; set; }
    }
}
