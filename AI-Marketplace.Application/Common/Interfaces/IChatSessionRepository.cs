using AI_Marketplace.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IChatSessionRepository
    {
        public Task<int> CreateSessionAsync(ChatSession session);
        public Task AddMessagesAsync(ICollection<ChatMessage> messages);
        public Task AddMessageAsync(ChatMessage message);
        public Task<ChatSession?> GetSessionByIdAsync(int SessionId);
        public Task<ICollection<ChatMessage>?> GetAllMessagesBySIdAsync(int SessionId);
        public Task<List<ChatSession>> GetSessionByUserIdAsync(int userId);

    }
}
