using CandidateManagement.Messaging; 

namespace CandidateManagement.Api.Services.Interfaces
{
    public interface IOutboxService
    {
        Task SaveMessageAsync(ActivityEvent activityEvent); 
    }
}
