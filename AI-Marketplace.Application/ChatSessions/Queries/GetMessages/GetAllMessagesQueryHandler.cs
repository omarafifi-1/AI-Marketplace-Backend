using AI_Marketplace.Application.ChatSessions.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
namespace AI_Marketplace.Application.ChatSessions.Queries.GetMessages
{
    public class GetAllMessagesQueryHandler:IRequestHandler<GetAllMessagesQuery,ICollection<GetMessageDTO>>
    {
        private IChatSessionRepository _chatSession;
        private IMapper _mapper;
        public GetAllMessagesQueryHandler(IChatSessionRepository chatSession, IMapper mapper)
        {
            this._chatSession = chatSession;
            this._mapper= mapper;
        }
        public async Task<ICollection<GetMessageDTO>> Handle(GetAllMessagesQuery query, CancellationToken cancellationToken)
        {
            ICollection<ChatMessage> Messages = await _chatSession.GetAllMessagesBySIdAsync(query.SessionId);
            if(Messages == null)
            {
                return null;
            }
            else
            {
                return _mapper.Map<ICollection<GetMessageDTO>>(Messages);
             }
        }

    }
}
