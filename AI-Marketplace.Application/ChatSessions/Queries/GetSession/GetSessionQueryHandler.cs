using AI_Marketplace.Application.ChatSessions.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Common.Mappings;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Queries.GetSession
{
    public class GetSessionQueryHandler : IRequestHandler<GetSessionQuery, GetSessionDTO>
    {
        private IChatSessionRepository _chatSession;
        private IMapper _mapper;
        public GetSessionQueryHandler(IChatSessionRepository chatSession, IMapper mapper) 
        {
            this._chatSession = chatSession;
            this._mapper = mapper;
        }

        public async Task<GetSessionDTO?> Handle(GetSessionQuery query, CancellationToken cancelToken)
        {
            ChatSession session= await _chatSession.GetSessionByIdAsync(query.SessionId);
            if (session == null)
            {
                return null;
            }
            else
            {
                return _mapper.Map<GetSessionDTO>(session);
            }
        }
    }
}
