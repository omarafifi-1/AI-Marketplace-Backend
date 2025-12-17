using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.DTOs
{
    public class AddMessageDTO
    {
        public int SessionId {  get; set; }
        [Required]
        public string Content { get; set; }
        public string Role { get; set; }   //user || assistant-> DEFAULT= string.empty
        public DateTime StartAt {  get; set; }
        public DateTime EndAt { get; set; }
    }
}
