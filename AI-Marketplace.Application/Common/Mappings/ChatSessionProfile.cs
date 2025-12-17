using AI_Marketplace.Application.ChatSessions.DTOs;
using AI_Marketplace.Application.Products.DTOs;
using AI_Marketplace.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Mappings
{
    internal class ChatSessionProfile: Profile
    {
        public ChatSessionProfile()
        {
            CreateMap<ChatMessage, GetMessageDTO>()
                /*.ForMember(dest => dest.SessionId,
                opt => opt.MapFrom(src => src.ChatSessionId))*/
                .ForMember(dest => dest.EndAt,
                opt => opt.MapFrom(src => src.Timestamp));
            CreateMap<AddMessageDTO, ChatMessage>().
                ForMember(dest => dest.ChatSessionId, opt => opt.MapFrom(src => src.SessionId)).
                ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.EndAt)).
                ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<ChatSession, GetSessionDTO>()
                .ForMember(dest => dest.SessionId,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EndAt,
                opt => opt.MapFrom(src => src.LastMessageAt)).
                ForMember(dest => dest.StartAt,
                opt => opt.MapFrom(src => src.StartedAt));
            CreateMap<AddSessionDTO, ChatSession>().
                ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartAt));
        }
    }
}
