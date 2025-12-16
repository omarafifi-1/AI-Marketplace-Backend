using AI_Marketplace.Application.ChatSessions.DTOs;
using AI_Marketplace.Application.ChatSessions.Queries.GetSession;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Application.Common.Mappings;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Queries.GetSessions
{
    public class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, List<GetSessionDTO>>
    {
        private IChatSessionRepository _chatSession;
        private IMapper _mapper;
        public GetSessionsQueryHandler(IChatSessionRepository chatSession, IMapper mapper) 
        {
            this._chatSession = chatSession;
            this._mapper = mapper;
        }

        public async Task<List<GetSessionDTO?>> Handle(GetSessionsQuery query, CancellationToken cancelToken)
        {
            List<ChatSession> sessions= await _chatSession.GetSessionByUserIdAsync(query.UserId);
            if (sessions == null)
            {
                return null;
            }
            else
            {
                return _mapper.Map<List<GetSessionDTO?>>(sessions);
            }
        }
    }
}
