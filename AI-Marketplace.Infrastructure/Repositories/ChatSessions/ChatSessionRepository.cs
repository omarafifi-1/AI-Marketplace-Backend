using AI_Marketplace.Application.Common.Interfaces;
using AI_Marketplace.Domain.Entities;
using AI_Marketplace.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Infrastructure.Repositories.ChatSessions
{ 
    internal class ChatSessionRepository: IChatSessionRepository
    {
        private readonly ApplicationDbContext _context;
        public ChatSessionRepository(ApplicationDbContext context)
        {
            this._context = context;
        }
        public async Task<int> CreateSessionAsync(ChatSession session) 
        {
            if (session == null) throw new ArgumentNullException("Empty Entry Data");
            else
            {
                await _context.ChatSessions.AddAsync(session);
                await _context.SaveChangesAsync();
                return session.Id;
            }
        }
        public async Task AddMessagesAsync(ICollection<ChatMessage> messages) 
        {
            if (messages == null) throw new ArgumentNullException("Empty Entry list");
            else
            {
                _context.ChatMessages.AddRange(messages);
                _context.SaveChanges();
            }
        }
        public async Task AddMessageAsync(ChatMessage message)
        {
            if (message == null) throw new ArgumentNullException("Empty Entry Data");
            await _context.ChatMessages.AddAsync(message);
            await _context.SaveChangesAsync();

        }
        public async Task<ChatSession?> GetSessionByIdAsync(int SessionId) 
        {
            return await _context.ChatSessions.
                Include(s=>s.ChatMessages).
                FirstOrDefaultAsync(s => s.Id == SessionId);
        }
        public async Task<ICollection<ChatMessage>?> GetAllMessagesBySIdAsync(int SessionId) 
        {
            List<ChatMessage>? messags= await _context.ChatMessages.Where(m=>m.ChatSessionId == SessionId).ToListAsync();
            if (messags == null) throw new KeyNotFoundException("No message Found with this Id");
            else 
            {
                return messags;
            }
        }
        public async Task<List<ChatSession>> GetSessionByUserIdAsync(int userId)
        {
            return await _context.ChatSessions.Where(s => s.UserId == userId).Include(s=>s.ChatMessages).OrderByDescending(s=>s.StartedAt).ToListAsync();
        }
    }
}
