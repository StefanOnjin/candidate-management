using CandidateManagement.Api.Models;

namespace CandidateManagement.Api.Repositories.Interfaces
{
    public interface IOutboxRepository
    {
        Task<List<OutboxMessage>> GetPendingAsync(int batchSize);

        Task AddAsync(OutboxMessage outboxMessage);

        void Update(OutboxMessage outboxMessage);

        Task SaveChangesAsync();
    }
}
