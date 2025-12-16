using AI_Marketplace.Application.ChatSessions.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Commands.AddSession
{
    public class AddSessionCommand: IRequest<int>
    {
        public AddSessionDTO SessionDTO { get; set; }
    }
}
