using AI_Marketplace.Application.ChatSessions.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Queries.GetSession
{
    public class GetSessionsQuery : IRequest<List<GetSessionDTO>>
    {
        public int UserId { get; set; }
    }
}
