namespace CandidateManagement.Api.Services.Interfaces
{
    public interface ITransactionManager
    {
        Task ExecuteAsync(Func<Task> action);

        Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    }
}
