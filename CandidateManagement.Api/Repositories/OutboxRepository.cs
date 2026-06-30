using CandidateManagement.Api.Data;
using CandidateManagement.Api.Models;
using CandidateManagement.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CandidateManagement.Api.Repositories
{
    public class OutboxRepository : IOutboxRepository
    {
        private readonly AppDbContext _context;

        public OutboxRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OutboxMessage>> GetPendingAsync(int batchSize)
        {
            return await _context.OutboxMessages
                .Where(m => m.ProcessedAtUtc == null)
                .OrderBy(m => m.OccurredAtUtc)
                .Take(batchSize)
                .ToListAsync();
        }

        public async Task AddAsync(OutboxMessage outboxMessage)
        {
            await _context.OutboxMessages.AddAsync(outboxMessage);
        }

        public void Update(OutboxMessage outboxMessage)
        {
            _context.OutboxMessages.Update(outboxMessage);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
