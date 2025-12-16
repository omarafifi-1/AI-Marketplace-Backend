using AI_Marketplace.Application.ChatSessions.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Commands.AddMessages
{
    public class AddMessagesCommand: IRequest
    {
        public ICollection<AddMessageDTO> messagesDTO;
    }
}
