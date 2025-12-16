using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Commands.AddSession
{
    public class AddSessionCommandHandler:IRequestHandler<AddSessionCommand, int>
    {
        private IChatSessionRepository _chatSession;
        private IMapper _mapper;
        public AddSessionCommandHandler(IChatSessionRepository chatSession, IMapper mapper) 
        {
            _chatSession = chatSession;
            _mapper = mapper;
        }
        public async Task<int> Handle(AddSessionCommand command, CancellationToken ct) 
        {
            if (command.SessionDTO == null)
            {
                throw new ArgumentNullException("Empty Entry");
            }
            else
            {
                ChatSession session= _mapper.Map<ChatSession>(command.SessionDTO);
                return await _chatSession.CreateSessionAsync(session);
            }
        }
    }
}
