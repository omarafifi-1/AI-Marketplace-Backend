using AI_Marketplace.Application.ChatSessions.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Queries.GetMessages
{
    public class GetAllMessagesQuery: IRequest<ICollection<GetMessageDTO>>
    {
        public int SessionId { get; set; }
    }
}
