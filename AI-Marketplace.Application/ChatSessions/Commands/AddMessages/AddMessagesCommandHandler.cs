using AI_Marketplace.Application.ChatSessions.DTOs;
using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace AI_Marketplace.Application.ChatSessions.Commands.AddMessages
{
    public class AddMessagesCommandHandler:IRequestHandler<AddMessagesCommand>
    {
        private IChatSessionRepository _chatSession;
        private IMapper _mapper;
        public AddMessagesCommandHandler(IChatSessionRepository chatSession, IMapper mapper)
        {
            this._mapper= mapper;
            this._chatSession= chatSession;
        }
        public async Task Handle(AddMessagesCommand command, CancellationToken ct)
        {
            if (command.messagesDTO == null)
            {
                throw new ArgumentNullException("Empty Entry");
            }
            else
            {
                ICollection<ChatMessage> messages = _mapper.Map<ICollection<ChatMessage>>(command.messagesDTO);
                await _chatSession.AddMessagesAsync(messages);
            }   
        }
    }
}
