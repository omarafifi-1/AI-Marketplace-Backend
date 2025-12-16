using AI_Marketplace.Application.ChatSessions.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Queries.GetSession
{
    public class GetSessionQuery : IRequest<GetSessionDTO>
    {
        public int SessionId { get; set; }
    }
}
