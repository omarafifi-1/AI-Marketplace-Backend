using AI_Marketplace.Application.ChatSessions.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Commands.AddMessages
{
    public class AddMessageCommandHandler: IRequestHandler<AddMessageCommand>
    {
        private IChatSessionRepository _chatSession;
        private IMapper _mapper;
        public AddMessageCommandHandler(IChatSessionRepository chatSession, IMapper mapper)
        {
            _chatSession = chatSession;
            _mapper = mapper;
        }
        public async Task Handle(AddMessageCommand command, CancellationToken ct) 
        {
            if(command.MessageDTO==null)
            {
                throw new ArgumentNullException("Empty Entry");
            }
            else
            {
                ChatMessage msg = _mapper.Map<ChatMessage>(command.MessageDTO);
                await _chatSession.AddMessageAsync(msg);
            }
        }
    }
}
