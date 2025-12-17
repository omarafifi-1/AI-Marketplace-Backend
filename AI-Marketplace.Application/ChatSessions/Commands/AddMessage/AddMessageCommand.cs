using System;
using System.Collections.Generic;
using System.Text;
using AI_Marketplace.Application.ChatSessions.DTOs;
using MediatR;

namespace AI_Marketplace.Application.ChatSessions.Commands.AddMessages
{
    public class AddMessageCommand:IRequest
    {
        public AddMessageDTO MessageDTO { get; set; }
    }
}
