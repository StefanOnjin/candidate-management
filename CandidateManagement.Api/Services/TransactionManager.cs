using CandidateManagement.Api.Data;
using CandidateManagement.Api.Services.Interfaces;

namespace CandidateManagement.Api.Services
{
    public class TransactionManager : ITransactionManager
    {
        private readonly AppDbContext _context;

        public TransactionManager(AppDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await action();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
